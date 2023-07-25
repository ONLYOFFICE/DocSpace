import React from "react";
import styled from "styled-components";
import { Outlet } from "react-router-dom";

import Layout from "SRC_DIR/Layout";

const MainWrapper = styled.div`
  width: 100%;
  height: 100vh;

  display: flex;
  flex-direction: row;
  box-sizing: border-box;
`;

const Client = () => {
  return (
    <MainWrapper>
      <Layout key="1">
        <Outlet />
      </Layout>
    </MainWrapper>
  );
};

export default Client;
