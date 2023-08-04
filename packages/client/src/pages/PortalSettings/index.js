import React from "react";
import { Outlet, useLocation } from "react-router-dom";

import Layout from "./Layout";

import Panels from "SRC_DIR/components/FilesPanels";
import { generalRoutes } from "SRC_DIR/routes/general";

const Settings = () => {
  const location = useLocation();

  let isGeneralPage = false;

  generalRoutes.forEach((route) => {
    if (isGeneralPage) return;

    isGeneralPage = location.pathname.includes(route.path);
  });

  return isGeneralPage ? (
    <>
      <Layout key="1" isGeneralPage>
        <Panels />
      </Layout>
      <Outlet />
    </>
  ) : (
    <Layout key="1">
      <Panels />
      <Outlet />
    </Layout>
  );
};

export default Settings;
