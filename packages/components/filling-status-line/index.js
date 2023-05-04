import React from "react";
import styled from "styled-components";
import Accordion from "./accordion.js";
import { Data } from "./data.js";

const FillingStatusContainer = styled.div`
  width: 100%;
  max-width: 425px;
`;

const FillingStatusLine = () => {
  return (
    <FillingStatusContainer>
      {Data.map((data) => {
        return (
          <Accordion
            key={data.id}
            displayName={data.displayName}
            role={data.role}
            status={data.status}
            avatar={data.avatar}
            date={data.date}
          />
        );
      })}
    </FillingStatusContainer>
  );
};

export default FillingStatusLine;
