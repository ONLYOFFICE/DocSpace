import React, { useState } from "react";
import styled from "styled-components";

import InfoIcon from "PUBLIC_DIR/images/info.react.svg?url";

import RadioButtonGroup from "@docspace/components/radio-button-group";
import Label from "@docspace/components/label";

import { Hint } from "../styled-components";
import { useTranslation } from "react-i18next";

const Header = styled.p`
  font-weight: 600;
  margin-top: 22px;
  margin-bottom: 10px;

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

export const SSLVerification = ({ onChange, value }) => {
  const [isHintVisible, setIsHintVisible] = useState(false);
  const { t } = useTranslation(["Webhooks"]);

  const handleOnChange = (e) => {
    onChange({
      target: { name: e.target.name, value: e.target.value === "true" },
    });
  };

  const toggleHint = () =>
    setIsHintVisible((prevIsHintVisible) => !prevIsHintVisible);
  return (
    <Label
      text={
        <Header>
          {t("SSLVerification")}{" "}
          <StyledInfoIcon
            className="ssl-verification-tooltip"
            src={InfoIcon}
            alt="infoIcon"
            onClick={toggleHint}
          />
        </Header>
      }
    >
      <Hint isTooltip hidden={!isHintVisible} onClick={toggleHint}>
        {t("SSLHint")}
      </Hint>
      <RadioButtonGroup
        fontSize="13px"
        fontWeight="400"
        name="ssl"
        onClick={handleOnChange}
        options={[
          {
            id: "enable-ssl",
            label: t("EnableSSL"),
            value: "true",
          },
          {
            id: "disable-ssl",
            label: t("DisableSSL"),
            value: "false",
          },
        ]}
        selected={value ? "true" : "false"}
        width="100%"
        orientation="vertical"
        spacing="8px"
      />
    </Label>
  );
};
