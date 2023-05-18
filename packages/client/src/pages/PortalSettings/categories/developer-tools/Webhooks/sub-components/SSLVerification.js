import React, { useState } from "react";
import styled from "styled-components";

import InfoIcon from "PUBLIC_DIR/images/info.react.svg?url";

import RadioButtonGroup from "@docspace/components/radio-button-group";

import { Hint } from "../styled-components";
import { useTranslation } from "react-i18next";

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

export const SSLVerification = ({ onChange, value }) => {
  const [isHintVisible, setIsHintVisible] = useState(false);
  const { t } = useTranslation(["Webhooks"]);

  const handleOnChange = (e) => {
    onChange({ target: { name: e.target.name, value: e.target.value === "true" } });
  };

  const toggleHint = () => setIsHintVisible((prevIsHintVisible) => !prevIsHintVisible);
  return (
    <div>
      <Header>
        {t("SSLVerification", { ns: "Webhooks" })}{" "}
        <StyledInfoIcon src={InfoIcon} alt="infoIcon" onClick={toggleHint} />
      </Header>

      <InfoHint hidden={!isHintVisible} onClick={toggleHint}>
        {t("SSLHint", { ns: "Webhooks" })}
      </InfoHint>

      <RadioButtonGroup
        fontSize="13px"
        fontWeight="400"
        name="ssl"
        onClick={handleOnChange}
        options={[
          {
            label: t("EnableSSL", { ns: "Webhooks" }),
            value: "true",
          },
          {
            label: t("DisableSSL", { ns: "Webhooks" }),
            value: "false",
          },
        ]}
        selected={value ? "true" : "false"}
        width="100%"
        orientation="vertical"
        spacing="8px"
      />
    </div>
  );
};
