using AmsLight.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AmsLight.Controllers
{
    [Authorize]

    public class TpProfileController : Controller
    {
        private AmsDbContext db = new AmsDbContext();


        public ActionResult Details()
        {
            int tpId = Convert.ToInt32(System.Web.HttpContext.Current.User.Identity.Name);

            TrainingPartner trainingPartner = db.TrainingPartner.Find(tpId);
            if (trainingPartner == null)
            {
                return HttpNotFound();
            }
            return View(trainingPartner);
        }
        // GET: TrainingPartners/Edit/5
        public ActionResult Edit()
        {
            int tpId = Convert.ToInt32(System.Web.HttpContext.Current.User.Identity.Name);

            TrainingPartner trainingPartner = db.TrainingPartner.Find(tpId);
            if (trainingPartner == null)
            {
                return HttpNotFound();
            }
            return View(trainingPartner);
        }

        // POST: TrainingPartners/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TpName,Phone1,Phone2,City,State,AddressLine1,AddressLine2,Zip")] TrainingPartner trainingPartner)
        {
            int tpId = Convert.ToInt32(System.Web.HttpContext.Current.User.Identity.Name);
            if (ModelState.IsValid)
            {
                var tp = db.TrainingPartner.Find(tpId);
                tp.TpName = trainingPartner.TpName;
                tp.Phone1 = trainingPartner.Phone1;
                tp.Phone2 = trainingPartner.Phone2;
                tp.City = trainingPartner.City;
                tp.State = trainingPartner.State;
                tp.AddressLine1 = trainingPartner.AddressLine1;
                tp.AddressLine2 = trainingPartner.AddressLine2;
                tp.Zip = trainingPartner.Zip;
                db.Entry(tp).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details");
            }
            return View(trainingPartner);
        }


    }
}