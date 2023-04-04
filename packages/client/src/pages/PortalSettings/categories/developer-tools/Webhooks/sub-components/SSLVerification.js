import React, { useState } from "react";
import styled from "styled-components";

import InfoIcon from "PUBLIC_DIR/images/info.react.svg?url";

import RadioButtonGroup from "@docspace/components/radio-button-group";

import { Hint } from "../styled-components";

const Header = styled.h1`
  font-family: "Open Sans";
  font-weight: 600;
  font-size: 13px;
  line-height: 20px;

  margin-top: 20px;

  color: #333333;
  display: flex;
  align-items: center;

  img {
    margin-left: 4px;
  }
`;

const StyledInfoIcon = styled.img`
  :hover {
    cursor: pointer;
  }
`;

const InfoHint = styled(Hint)`
  position: absolute;
  z-index: 2;

  width: 320px;
`;

export const SSLVerification = () => {
  const [isHintVisible, setIsHintVisible] = useState(false);

  const toggleHint = () => setIsHintVisible((prevIsHintVisible) => !prevIsHintVisible);
  return (
    <div>
      <Header>
        SSL verification <StyledInfoIcon src={InfoIcon} alt="infoIcon" onClick={toggleHint} />
      </Header>

      <InfoHint hidden={!isHintVisible} onClick={toggleHint}>
        By default, we verify SSL certificates when delivering payloads.
      </InfoHint>

      <RadioButtonGroup
        fontSize="13px"
        fontWeight="400"
        name="group"
        onClick={() => {}}
        options={[
          {
            label: "Enable SSL verification",
            value: "enable",
          },
          {
            label: "Disable (not recommended)",
            value: "disable",
          },
        ]}
        selected="enable"
        width="100%"
        orientation="vertical"
        spacing="8px"
      />
    </div>
  );
};
