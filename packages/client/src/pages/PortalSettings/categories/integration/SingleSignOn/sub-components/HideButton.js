import React from "react";
import styled, { css } from "styled-components";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Link from "@docspace/components/link";
import Text from "@docspace/components/text";

const StyledWrapper = styled.div`
  display: flex;
  flex-direction: row;
  align-items: center;
  margin: ${(props) => (props.isAdditionalParameters ? "0" : "24px 0")};

  .hide-button {
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin-right: 12px;
          `
        : css`
            margin-left: 12px;
          `}
  }
`;

const HideButton = (props) => {
  const { t } = useTranslation("SingleSignOn");
  const {
    text,
    label,
    isAdditionalParameters,
    value,
    setHideLabel,
    isDisabled,
    id,
  } = props;

  const onClick = () => {
    setHideLabel(label);
  };

  const onClickProp = isDisabled ? {} : { onClick: onClick };

  return (
    <StyledWrapper isAdditionalParameters={isAdditionalParameters}>
      {!isAdditionalParameters && (
        <Text
          as="h2"
          fontSize="16px"
          fontWeight={700}
          className="settings_unavailable"
          noSelect
        >
          {text}
        </Text>
      )}

      <Link
        id={id}
        className="hide-button settings_unavailable"
        isHovered
        {...onClickProp}
        type="action"
      >
        {value
          ? isAdditionalParameters
            ? t("HideAdditionalParameters")
            : t("Hide")
          : isAdditionalParameters
          ? t("ShowAdditionalParameters")
          : t("Show")}
      </Link>
    </StyledWrapper>
  );
};

export default inject(({ ssoStore }) => {
  const { setHideLabel } = ssoStore;

  return {
    setHideLabel,
  };
})(observer(HideButton));
