import React from "react";
import Accordion from "./accordion.js";

import DoneReactSvg from "PUBLIC_DIR/images/done.react.svg";
import { StyledFillingStatusContainer } from "./styled-filling-status-line.js";
import Text from "../text";
import Box from "../box";
import { Data } from "./data.js";

const FillingStatusLine = () => {
  return (
    <StyledFillingStatusContainer>
      {Data.map((data) => {
        return (
          <Accordion
            key={data.id}
            displayName={data.displayName}
            role={data.role}
            status={data.status}
            avatar={data.avatar}
            date={data.date}
            finished={data.finished}
          />
        );
      })}
      <Box displayProp="flex" alignItems="center" marginProp="15px 0 0">
        <DoneReactSvg className="done-icon" />
        <Text
          fontSize="14px"
          lineHeight="16px"
          color="#A3A9AE"
          fontWeight="bold"
        >
          Done
        </Text>
      </Box>
    </StyledFillingStatusContainer>
  );
};

export default FillingStatusLine;
