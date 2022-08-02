import React, { useEffect } from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import { withRouter } from "react-router";
import Text from "@docspace/components/text";
import { useTranslation, Trans } from "react-i18next";
import moment from "moment";
import { inject, observer } from "mobx-react";

const StyledHeader = styled.div`
  display: grid;
  grid-template-columns: 1fr;
  grid-template-rows: min-content min-content;
  grid-row-gap: 8px;
  .payments-header {
    font-style: normal;
    line-height: 32px;
  }
  .payments-header-additional_support {
    margin-bottom: 40px;
    line-height: 20px;
  }
  @media (max-width: 632px) {
    .payments-header-additional_support {
      margin-bottom: 16px;
    }
  }
`;

const HeaderContainer = ({
  theme,
  culture,
  trialMode,
  expiresDate,
  organizationName,
}) => {
  useEffect(() => {
    const moment = require("moment");
    require("moment/min/locales.min");
    culture && moment.locale(culture);
  }, [culture]);

  const { t } = useTranslation("PaymentsEnterprise");

  const now = moment();

  const licenseDate = moment(expiresDate);
  const licenseDateString = licenseDate.startOf("day").format("D MMMM, YYYY");

  return licenseDate.isAfter(now, "day") ? (
    <StyledHeader>
      <Text className="payments-header" fontSize="27px" isBold={true}>
        <Trans t={t} i18nKey="HeaderLicense" ns="PaymentsEnterprise">
          {{ organizationName }}
        </Trans>
      </Text>
      <Text className="payments-header-additional_support">
        {t("AccessSubscription")} {licenseDateString}
        {"."}
      </Text>
    </StyledHeader>
  ) : !trialMode ? (
    <StyledHeader>
      <Text className="payments-header" fontSize="27px" isBold={true}>
        <Trans t={t} i18nKey="HeaderLicense" ns="PaymentsEnterprise">
          {{ organizationName }}
        </Trans>
      </Text>
      <Text
        className="payments-header-additional_support"
        color={theme.client.paymentsEnterprise.headerColor}
        fontWeight="600"
      >
        {t("ExpiryPaidLicense")} {licenseDateString}
        {". "}
        {t("RenewalLicense")}
      </Text>
    </StyledHeader>
  ) : (
    <StyledHeader>
      <Text className="payments-header" fontSize="27px" isBold={true}>
        {t("HeaderExpiredTrialLicense")}
      </Text>
      <Text className="payments-header-additional_support">
        <Trans t={t} i18nKey="ExpiryTrialLicense" ns="PaymentsEnterprise">
          {{ organizationName }}
        </Trans>
      </Text>
    </StyledHeader>
  );
};

HeaderContainer.propTypes = {
  culture: PropTypes.string,
  expiresDate: PropTypes.object,
  trialMode: PropTypes.bool,
};

export default inject(({ auth, payments }) => {
  const { organizationName, culture, theme } = auth.settingsStore;
  const { expiresDate, trialMode } = payments;
  return {
    theme,
    organizationName,
    culture,
    expiresDate,
    trialMode,
  };
})(withRouter(observer(HeaderContainer)));
