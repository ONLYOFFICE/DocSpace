import React from "react";
import { Outlet } from "react-router-dom";

import Layout from "SRC_DIR/Layout";

const Client = () => {
  return (
    <Layout>
      <Outlet />
    </Layout>
  );
};

export default Client;
