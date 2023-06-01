import React from "react";
import { Outlet } from "react-router-dom";
import Layout from "./Layout";

import Panels from "../../components/FilesPanels";

const Settings = () => {
  return (
    <Layout key="1">
      <Panels />
      <Outlet />
    </Layout>
  );
};

export default Settings;
