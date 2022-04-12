using System.Collections.Generic;
using SS.HealthApp.ClientConnector.Interfaces;
using SS.HealthApp.ClientConnector.Models;
using SS.HealthApp.Model;
using SS.HealthApp.Model.AppointmentModels;
using System;
using System.Linq;
using System.Globalization;

namespace SS.HealthApp.ClientConnector.SAMS
{
    public class AppointmentClientConnector : IAppointmentClientConnector
    {

        public List<Appointment> GetAppointments(AuthenticatedUser User)
        {

            var appts = new List<Appointment>();

            using (MySAMSApiWS.MySAMSApiWSClient serviceProxy = new MySAMSApiWS.MySAMSApiWSClient())
            {
                serviceProxy.ClientCredentials.UserName.UserName = Config.MySAMSApiWS_Username;
                serviceProxy.ClientCredentials.UserName.Password = Config.MySAMSApiWS_Password;

                MySAMSApiWS.Consulta[] items = serviceProxy.ObterListaConsultas(User.Id);

                if (items != null)
                {
                    foreach (var item in items)
                    {
                        if (item.Estado.ToLower() != "cancelado")
                        {
                            var apt = new Appointment() {
                                ID = item.IdConsulta,
                                Description = item.Acto,
                                Facility = item.UnidadeSaude,
                                Moment = item.Momento,
                                Status = (item.Estado.ToLower() == "realizado" || item.Estado.ToLower() == "transferido"
                                                ? Model.Enum.AppointmentStatus.Closed : Model.Enum.AppointmentStatus.Booked)
                            };

                            if (apt.Status == Model.Enum.AppointmentStatus.Booked) 
                                apt.AllowAddToCalendar = apt.AllowCancel = true;

                            appts.Add(apt);
                        }
                    }
                }
            }
            return appts;
        }

        public AppointmentData GetAllData()
        {

            var ApptData = new AppointmentData();

            using (MySAMSApiWS.MySAMSApiWSClient serviceProxy = new MySAMSApiWS.MySAMSApiWSClient())
            {
                serviceProxy.ClientCredentials.UserName.UserName = Config.MySAMSApiWS_Username;
                serviceProxy.ClientCredentials.UserName.Password = Config.MySAMSApiWS_Password;

                MySAMSApiWS.DadosAgendamento dados = serviceProxy.ObterDadosAgendamento();

                if (dados != null)
                {

                    foreach (var item in dados.Doctors)
                        ApptData.Doctors.Add(new PickerItem(item.Id, item.Designacao));

                    foreach (var item in dados.Specialties)
                        ApptData.Specialties.Add(new PickerItem(item.Id, item.Designacao));

                    foreach (var item in dados.Services)
                    {

                        //Could use this approach but if a new type is created this stops working
                        //var type = (Enum.AppointmentType)System.Enum.Parse(typeof(Enum.AppointmentType), item.Tipo);

                        Model.Enum.AppointmentType? type = null;

                        if (item.Tipo.ToUpper() == Model.Enum.AppointmentType.C.ToString())
                            type = Model.Enum.AppointmentType.C;
                        else if (item.Tipo.ToUpper() == Model.Enum.AppointmentType.E.ToString())
                            type = Model.Enum.AppointmentType.E;
                        else if (item.Tipo.ToUpper() == Model.Enum.AppointmentType.O.ToString())
                            type = Model.Enum.AppointmentType.O;

                        if (type != null)
                            ApptData.Services.Add(new ServiceItem(item.Id, item.Designacao, (Model.Enum.AppointmentType)type));
                    }

                    foreach (var item in dados.Facilities)
                        ApptData.Facilities.Add(new PickerItem(item.Id, item.Designacao));

                    foreach (var item in dados.Relations)
                    {
                        var doctor = ApptData.Doctors.Find(d => d.ID == item.DoctorID);
                        var specialty = ApptData.Specialties.Find(s => s.ID == item.SpecialtyID);
                        var service = ApptData.Services.Find(s => s.ID == item.ServiceID);
                        var facility = ApptData.Facilities.Find(f => f.ID == item.FacilityID);
                        if (specialty != null && service != null && facility != null)
                            ApptData.Relations.Add(new AppointmentData.Relation(doctor, specialty, service, facility));
                    }
                }
            }

            return ApptData;
        }

        public List<PickerItem> GetAvailableDates(AuthenticatedUser User, AppointmentBook apptBook)
        {

            var availableDates = new Dictionary<string, PickerItem>();
            CultureInfo ci = new CultureInfo("pt-PT");

            using (MySAMSApiWS.MySAMSApiWSClient serviceProxy = new MySAMSApiWS.MySAMSApiWSClient())
            {
                serviceProxy.ClientCredentials.UserName.UserName = Config.MySAMSApiWS_Username;
                serviceProxy.ClientCredentials.UserName.Password = Config.MySAMSApiWS_Password;

                string payor = null, plan = null;
                //if (apptBook.Payor != null && !string.IsNullOrEmpty(apptBook.Payor.ID)) {
                //    string[] payordata = apptBook.Payor.ID.Split('_');
                //    payor = payordata[0];
                //    plan = payordata[1];
                //}

                MySAMSApiWS.DataHoraAgendamentoItem[] items = serviceProxy.ObterDatasDisponiveisAgendamento(
                    User.Id,
                    apptBook.Facility.ID,
                    apptBook.Specialty.ID,
                    apptBook.Doctor != null ? apptBook.Doctor.ID : null,
                    apptBook.Service.ID,
                    apptBook.Moment.ToString("yyyy-MM-dd"),
                    payor,
                    plan);


                if (items != null)
                {

                    foreach (var item in items)
                    {
                        DateTime date;

                        if (DateTime.TryParse(item.Designacao, out date))
                        {

                            PickerItem pItem = new PickerItem(item.Id, date.ToString("dd/MMM/yyyy", ci));

                            if (!availableDates.ContainsKey(pItem.Title))
                            {
                                //only add the date if is not already in the return collection
                                //If no resource selected, we can have multiple resources with availability on same days
                                //which results in duplicated dates in results
                                availableDates.Add(pItem.Title, pItem);
                            }
                        }
                    }

                }
            }

            return availableDates.Values.OrderBy(pi => DateTime.ParseExact(pi.Title, "dd/MMM/yyyy", ci)).ToList();
        }

        public List<PickerItem> GetAvailableTime(AuthenticatedUser User, AppointmentBook apptBook)
        {
            var availableTime = new Dictionary<string, PickerItem>();

            using (MySAMSApiWS.MySAMSApiWSClient serviceProxy = new MySAMSApiWS.MySAMSApiWSClient())
            {
                serviceProxy.ClientCredentials.UserName.UserName = Config.MySAMSApiWS_Username;
                serviceProxy.ClientCredentials.UserName.Password = Config.MySAMSApiWS_Password;

                string payor = null, plan = null;
                //if (apptBook.Payor != null && !string.IsNullOrEmpty(apptBook.Payor.ID)) {
                //    string[] payordata = apptBook.Payor.ID.Split('_');
                //    payor = payordata[0];
                //    plan = payordata[1];
                //}

                if (apptBook.Doctor != null && !String.IsNullOrEmpty(apptBook.Doctor.ID))
                {
                    //the doctor is specified, call the ObterHorasDisponiveisAgendamento and return the result
                    MySAMSApiWS.DataHoraAgendamentoItem[] items = serviceProxy.ObterHorasDisponiveisAgendamento(
                            User.Id,
                            apptBook.Facility.ID,
                            apptBook.Specialty.ID,
                            apptBook.Doctor.ID,
                            apptBook.Service.ID,
                            apptBook.Moment.ToString("yyyy-MM-dd"), 
                            payor, 
                            plan);

                    return items.Select(i => new PickerItem(i.Id, i.Designacao)).ToList();

                }

                //the doctor is not specified
                //get all doctors(resources) that have availability for the selected day
                //return all the slot available from all resorces
                //if multiple resources have availability at the same slot hour, return the slot associated with the first resource

                string selectedDate = apptBook.Moment.ToString("yyyy-MM-dd");

                //get all doctors(resources) that have availability for the selected day
                MySAMSApiWS.DataHoraAgendamentoItem[] itemsDate = serviceProxy.ObterDatasDisponiveisAgendamento(
                    User.Id,
                    apptBook.Facility.ID,
                    apptBook.Specialty.ID,
                    null,
                    apptBook.Service.ID,
                    selectedDate,
                    payor,
                    plan);


                if (itemsDate != null)
                {
                    var allResources = itemsDate.Where(iDate => iDate.Id == selectedDate)
                                                .Select(iDate => iDate.IdRecurso != null ? iDate.IdRecurso.Trim() : iDate.IdRecurso ) //need to trim IdRecurso, service does not return null, returns " " when no resource is associated with the availability entry
                                                .Distinct();

                    //get the availability foreach resource
                    foreach (var resourceItem in allResources)
                    {
                        MySAMSApiWS.DataItem[] items = serviceProxy.ObterHorasDisponiveisAgendamento(
                            User.Id,
                            apptBook.Facility.ID,
                            apptBook.Specialty.ID,
                            resourceItem,
                            apptBook.Service.ID,
                            selectedDate,
                            payor,
                            plan);

                        if (items != null)
                        {
                            foreach (var item in items)
                            {
                                if (!availableTime.ContainsKey(item.Designacao))
                                {
                                    //if the slot already in return collection ignore it(multiple resources have availability at the same slot hour, return the first one)
                                    availableTime.Add(item.Designacao, new PickerItem(item.Id, item.Designacao));
                                }

                            }
                        }
                    }

                }
            }

            //return ordered by hour ASC
            return availableTime.Values.OrderBy(pi => pi.Title).ToList();
        }

        public List<PickerItem> GetPayors(AuthenticatedUser User)
        {

            var payors = new List<PickerItem>();

            using (MySAMSApiWS.MySAMSApiWSClient serviceProxy = new MySAMSApiWS.MySAMSApiWSClient())
            {
                serviceProxy.ClientCredentials.UserName.UserName = Config.MySAMSApiWS_Username;
                serviceProxy.ClientCredentials.UserName.Password = Config.MySAMSApiWS_Password;

                MySAMSApiWS.OrganismoPlano[] items = serviceProxy.ObterEntidadesAgendamento(User.Id);

                if (items != null)
                {
                    foreach (var item in items)
                    {
                        payors.Add(new PickerItem(string.Format("{0}_{1}", item.InsuranceID, item.PlanID), item.InsuranceDesc));
                    }
                }
            }

            return payors;
        }

        public bool CancelAppointment(AuthenticatedUser User, string appointmentID)
        {

            using (MySAMSApiWS.MySAMSApiWSClient serviceProxy = new MySAMSApiWS.MySAMSApiWSClient())
            {
                serviceProxy.ClientCredentials.UserName.UserName = Config.MySAMSApiWS_Username;
                serviceProxy.ClientCredentials.UserName.Password = Config.MySAMSApiWS_Password;

                return serviceProxy.CancelarAgendamento(User.Id, appointmentID);
            }

        }

        public bool BookNewAppointment(AuthenticatedUser User, AppointmentBook apptBook)
        {

            using (MySAMSApiWS.MySAMSApiWSClient serviceProxy = new MySAMSApiWS.MySAMSApiWSClient())
            {
                serviceProxy.ClientCredentials.UserName.UserName = Config.MySAMSApiWS_Username;
                serviceProxy.ClientCredentials.UserName.Password = Config.MySAMSApiWS_Password;

                string[] payordata = apptBook.Payor.ID.Split('_');
                return serviceProxy.Agendar(User.Id, apptBook.SlotID, apptBook.Service.ID, payordata[0], payordata[1]);
            }

        }
    }
}
