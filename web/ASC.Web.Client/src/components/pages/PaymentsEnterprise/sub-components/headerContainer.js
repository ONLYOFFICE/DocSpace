import React, { useEffect } from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import { Text } from "asc-web-components";
import { utils } from "asc-web-common";

import { useTranslation, Trans } from "react-i18next";
import { createI18N } from "../../../../helpers/i18n";
import moment from "moment";
const { changeLanguage } = utils;

const i18n = createI18N({
  page: "PaymentsEnterprise",
  localesPath: "pages/PaymentsEnterprise",
});

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
  culture,
  trialMode,
  expiresDate,
  organizationName,
}) => {
  useEffect(() => {
    changeLanguage(i18n);
    const moment = require("moment");
    require("moment/min/locales.min");
    culture && moment.locale(culture);
  }, [culture]);

  const { t } = useTranslation("translation", { i18n });

  const now = moment();

  const licenseDate = moment(expiresDate);
  const licenseDateString = licenseDate.startOf("day").format("D MMMM, YYYY");

  return licenseDate.isAfter(now, "day") ? (
    <StyledHeader>
      <Text className="payments-header" fontSize="27px" isBold={true}>
        <Trans i18nKey="HeaderLicense" i18n={i18n}>
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
        <Trans i18nKey="HeaderLicense" i18n={i18n}>
          {{ organizationName }}
        </Trans>
      </Text>
      <Text
        className="payments-header-additional_support"
        color="#C96C27"
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
        <Trans i18nKey="ExpiryTrialLicense" i18n={i18n}>
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
function mapStateToProps({ auth, payments }) {
  const { culture, organizationName } = auth.settings;
  const { expiresDate, trialMode } = payments.currentLicense;
  return {
    culture,
    expiresDate,
    trialMode,
    organizationName,
  };
}
export default connect(mapStateToProps)(withRouter(HeaderContainer));
