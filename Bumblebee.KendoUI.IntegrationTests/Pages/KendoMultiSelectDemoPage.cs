﻿using System;

using Bumblebee.Implementation;
using Bumblebee.Interfaces;
using Bumblebee.Setup;

using OpenQA.Selenium;

namespace Bumblebee.KendoUI.IntegrationTests.Pages
{
    public class KendoMultiSelectDemoPage : WebBlock
    {
        public KendoMultiSelectDemoPage(Session session)
            : base(session, TimeSpan.FromSeconds(10))
        {
        }

        public ISelectBox<KendoMultiSelectDemoPage> Movies
        {
            get { return new KendoMultiSelect<KendoMultiSelectDemoPage>(this, By.Id("movies")); }
        }
    }
}
