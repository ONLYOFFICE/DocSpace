import React from "react";
import styled from "styled-components";
import SmartBanner from "react-smartbanner";
import "./smartbanner.css";

const Wrapper = styled.div`
  padding-bottom: 80px;
`;

const ReactSmartBanner = () => {
  return (
    <Wrapper>
      <SmartBanner title="Onlyoffice" author="Onlyoffice" />
    </Wrapper>
  );
};

export default ReactSmartBanner;
