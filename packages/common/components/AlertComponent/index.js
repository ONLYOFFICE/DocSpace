import React from "react";
import { inject, observer } from "mobx-react";
import { withRouter } from "react-router";
import styled from "styled-components";

import Text from "@docspace/components/text";
import commonIconsStyles from "@docspace/components/utils/common-icons-style";

import ArrowRightIcon from "PUBLIC_DIR/images/arrow.right.react.svg";
import CrossReactSvg from "PUBLIC_DIR/images/cross.react.svg";

import Loaders from "../Loaders";
import { StyledAlertComponent } from "./StyledComponent";

const StyledArrowRightIcon = styled(ArrowRightIcon)`
  margin: auto 0;
  path {
    fill: ${(props) => props.color};
  }
`;
const StyledCrossIcon = styled(CrossReactSvg)`
  position: absolute;
  right: 0px;
  margin-right: 8px;
  margin-top: 8px;

  ${commonIconsStyles}
  path {
    fill: ${(props) => props.color};
  }
`;

const AlertComponent = ({
  id,
  description,
  title,
  titleFontSize,
  additionalDescription,
  needArrowIcon = false,
  needCloseIcon = false,
  link,
  linkColor,
  linkTitle,
  onAlertClick,
  onCloseClick,
  mainColor,
}) => {
  return (
    <StyledAlertComponent
      mainColor={mainColor}
      onClick={onAlertClick}
      needArrowIcon={needArrowIcon}
      id={id}
    >
      <div>
        <Text
          className="alert-component_title"
          fontSize={titleFontSize ?? "12px"}
          fontWeight={600}
        >
          {title}
        </Text>
        {additionalDescription && (
          <Text fontWeight={600}>{additionalDescription}</Text>
        )}
        <Text noSelect fontSize="12px">
          {description}
        </Text>
        {link && (
          <Link type="page" href={link} noHover color={linkColor} title={email}>
            {linkTitle}
          </Link>
        )}
      </div>
      {needCloseIcon && (
        <StyledCrossIcon size="small" onCloseClick={onCloseClick} />
      )}
      {needArrowIcon && (
        <StyledArrowRightIcon className="alert-component_arrow" />
      )}
    </StyledAlertComponent>
  );
};

export default withRouter(
  inject(({ auth }) => {
    const { paymentQuotasStore, currentQuotaStore, settingsStore } = auth;
    const { currentTariffPlanTitle } = currentQuotaStore;

    const {
      setPortalPaymentQuotas,
      planCost,
      tariffPlanTitle,
    } = paymentQuotasStore;

    return {
      setPortalPaymentQuotas,
      pricePerManager: planCost.value,

      currencySymbol: planCost.currencySymbol,
      currentTariffPlanTitle,
      tariffPlanTitle,
    };
  })(observer(AlertComponent))
);
