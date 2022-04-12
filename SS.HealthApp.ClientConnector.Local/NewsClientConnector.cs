using System;
using System.Collections.Generic;
using System.Linq;
using SS.HealthApp.ClientConnector.Interfaces;
using SS.HealthApp.Model.NewsModels;


namespace SS.HealthApp.ClientConnector.Local
{
    public class NewsClientConnector : INewsClientConnector
    {

        static List<News> newsDataSource = FillNewsDataSource();

        public List<News> GetNews()
        {
            return newsDataSource.OrderByDescending(n => n.Date).ToList();
        }

        public News GetNews(string ID)
        {
            return newsDataSource.FirstOrDefault(item => item.ID == ID);
        }

        private static List<News> FillNewsDataSource()
        {
            return new List<News> {
                new News {
                    ID = "1",
                    Name = "The Future of Medicine is a Virtual World",
                    Image = string.Concat(Settings.ResourcesUrl, "News1.jpg"),
                    Date = new DateTime(2017,2,22),
                    Detail = "The future of surgery offers an amazing cooperation between humans and technology, which could elevate the level of precision and efficiency of surgeries higher than we have ever seen before."
                                + "\r\n\r\nWill we have Matrix-like small surgical robots? Will they pull in and out organs from patients’ bodies?"
                                + "\r\n\r\nThe scene is not impossible. It looks like we have come a long way from ancient Egypt, where doctors performed invasive surgeries as far back as 3,500 years ago. Only two years ago, NASA teamed up with American medical company Virtual Incision to develop a robot that can be placed inside a patient’s body and then controlled remotely by a surgeon."
                                + "\r\n\r\nThat’s the reason why I strongly believe surgeons have to reconsider their stance towards technology and the future of their profession."
                                + "\r\n\r\nSurgeons are at the top of the medical food chain. At least that’s the impression the general audience gets from popular medical drama series and their own experiences. No surprise there. Surgeons bear huge responsibilities: they might cause irreparable damages and medical miracles with one incision on the patient’s body. No wonder that with the rise of digital technologies, the Operating Rooms and surgeons are inundated with new devices aiming at making the least cuts possible."
                },
                new News {
                    ID = "2",
                    Name = "Artificial Intelligence is Leading a Revolution",
                    Image = string.Concat(Settings.ResourcesUrl, "News2.jpg"),
                    Date = new DateTime(2016,09,15),
                    Detail = "The success of this work will help healthcare professionals diagnose more accurately and efficiently, and it will allow for more diagnostic care in areas with limited healthcare services and providers."
                                + "\r\n\r\nIn early August, IBM announced that it will acquire Merge Healthcare Inc., a company that sells systems that help medical professionals access and store medical images. This move is a critical step in IBM’s plan to put AI to work medically by training its Watson software to identify maladies like heart disease and cancer."
                                + "\r\n\r\nMerge is valuable to IBM because it owns 30 billion images, including computerized tomography, X-rays, and magnetic-resonance-imaging scans. The company can use these images in its deep learning training program. IBM is hoping that the same kind of software that lets Flickr recognize your face or a dog in your photos can help Watson identify symptoms of diseases."
                                + "\r\n\r\nIf they’re right, IBM may someday be able to provide services that help streamline the diagnostic process for doctors, lowering costs and raising efficiency. This, in turn, could turn IBM into a major player in the $7.2 trillion global healthcare market."
                                + "\r\n\r\nPerhaps more interestingly, if IBM’s plan works, the countless medical images that are now sitting in storage will have a new purpose. As deep learning becomes more common in medicine, these images will be extremely valuable."
                },
                new News {
                    ID = "3",
                    Name = "Reverse Alzheimer’s in Mice",
                    Image = string.Concat(Settings.ResourcesUrl, "News3.jpg"),
                    Date = new DateTime(2016,08,17),
                    Detail = "A new paper reveals that a commonly used medication can reverse signs of Alzheimer's in mice, and it does so in as little as a month. If it can be translated to humans, we could have a promising new treatment for the disease (but that's a big if)."
                                + "\r\n\r\nScientists have discovered that the pain reliever mefenamic acid – usually given to help with period pain – has a notable side effect: it also reverses the symptoms of Alzheimer’s disease in mice."
                                + "\r\n\r\nMice with symptoms of Alzheimer’s were treated with doses of mefenamic acid for one month, and their memory loss and brain inflammation completely cleared up."
                                + "\r\n\r\nIf the same treatment can be translated to humans – although that’s a big if – then we could have a promising new treatment for Alzheimer’s disease on our hands."
                },
                new News {
                    ID = "4",
                    Name = "Regenerative Medicine",
                    Image = string.Concat(Settings.ResourcesUrl, "News4.jpg"),
                    Date = new DateTime(2016,08,16),
                    Detail = "Scientists have just found a way to make use of plasma, the fourth state of matter, to improve bone development. Using cold fusion, researchers were able to initiate increased bone growth."
                                + "\r\n\r\nIt is a bit ironic that plasma is the least known state of matter, when in fact it is the most abundant in the universe. It is found in our Sun and all other stars, lightning, in our TVs, fluorescent light, and neon signs, and (purportedly) even in our favorite fictional weapon in the Star Wars universe, the lightsaber."
                                + "\r\n\r\nPlasma can be classified according to the degree of ionization, temperature, etc, but whatever form it may take, plasma has been used in various fields, such as in spacecraft propulsion, agriculture, and quite recently, in medicine."
                                + "\r\n\r\nIn a study recently published in Journal of Tissue Engineering and Regenerative Medicine, a team of scientists avers that cold plasma could help improve bone development."
                },
                new News {
                    ID = "5",
                    Name = "Drones to Start Delivery of Medicine in Rural US",
                    Image = string.Concat(Settings.ResourcesUrl, "News5.jpg"),
                    Date = new DateTime(2016,08,09),
                    Detail = "The nation of Rwanda recently partnered with a drone company for drone delivery of medical supplies to far-off rural areas. Being the first time drones have been used to deliver medicine, the initiative cemented Rwanda’s commitment to drone technology and its many uses."
                                + "\r\n\r\nNow, the United States is looking to explore a similar deal. Zipline, the company the Rwandan government partnered with, is now in a program with the US government to deliver medical supplies to areas in the rural US."
                                + "\r\n\r\nZipline uses electric-powered drones, called “Zips.” The drones could carry up to three pounds of blood or medicine, and could be flown 75 miles on a single charge. Navigation is via GPS and cellular networks."
                                + "\r\n\r\nHospitals could order their goodie bags via text message and expect the supplies inside packages with parachutes. Like the best pizzerias, the drones will deliver in 30 minutes or less. The short trips mean the items do not require refrigeration."
                },
                new News {
                    ID = "6",
                    Name = "Nobel Prize Goes To “Self-Eating” Cells",
                    Image = string.Concat(Settings.ResourcesUrl, "News6.jpg"),
                    Date = new DateTime(2016,06,10),
                    Detail = "The Nobel prize in medicine was awarded to cellular biologist Yoshinori Ohsumi for his research in autophagy, a vital mechanism of biological cells by which they digest and recycle waste. Ohsumi’s work is a vital discovery in biomedicine that is the foundation to many forms of treatment."
                                + "\r\n\r\nAutophagy literally means “self-eating.” It was first observed in the 1960s, when researchers observed that living cells “recycled” its own waste, destroying subcellular parts of itself and transporting them to other regions of the cell. Studying these mechanisms was difficult, and very little was known about them until Ohsumi’s groundbreaking experiments in the early 1990s."
                                + "\r\n\r\nThe Japanese biologist used baker’s yeast to elucidate the mechanism of autophagy in yeast, identifying genes essential in the cell’s waste-processing system."
                }
            };
        }
        

    }
}
