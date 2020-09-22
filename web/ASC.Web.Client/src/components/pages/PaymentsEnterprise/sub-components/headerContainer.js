import React, { useEffect } from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import { Text, utils as Utils } from "asc-web-components";
import { utils } from "asc-web-common";

import { useTranslation } from "react-i18next";
import { createI18N } from "../../../../helpers/i18n";
import moment from "moment";
const { changeLanguage } = utils;
const { tablet } = Utils.device;

const i18n = createI18N({
  page: "PaymentsEnterprise",
  localesPath: "pages/PaymentsEnterprise",
});
const StyledHeader = styled.div`
  .payments-header {
    margin-top: 46px;
    font-style: normal;
    font-weight: bold;
    font-size: 27px;
    line-height: 32px;
    color: #333333;
    margin-bottom: 8px;
  }
  .payments-header-additional_support {
    margin-bottom: 40px;
    font-size: 13px;
    line-height: 20px;
  }

  @media ${tablet} {
    display: grid;
    .payments-header {
      margin-top: 39px;
      font-style: normal;
      font-weight: bold;
      font-size: 27px;
      line-height: 32px;
    }
  }

  @media (max-width: 632px) {
    .payments-header {
      margin-top: 1px;
    }
    .payments-header-additional_support {
      margin-bottom: 16px;
    }
  }
`;

const HeaderContainer = ({
  culture,
  utcHoursOffset,
  trialMode,
  expiresDate,
}) => {
  useEffect(() => {
    changeLanguage(i18n);
    const moment = require("moment");
    require("moment/min/locales.min");
    culture && moment.locale(culture);
  }, [culture]);

  const { t } = useTranslation("translation", { i18n });
  const currentUserDate = moment().utcOffset(utcHoursOffset);

  return new Date(currentUserDate).setHours(0, 0, 0, 0) <
    expiresDate.setHours(0, 0, 0, 0) ? (
    <StyledHeader>
      <Text className="payments-header">{t("Using")}</Text>
      <Text className="payments-header-additional_support">
        {t("SubscriptionAndUpdatesExpires")}{" "}
        {moment(expiresDate).startOf("day").format(" D MMMM, YYYY")}
        {"."}
      </Text>
    </StyledHeader>
  ) : !trialMode ? (
    <StyledHeader>
      <Text className="payments-header">{t("Using")}</Text>
      <Text
        className="payments-header-additional_support"
        color="#C96C27"
        fontWeight="600"
      >
        {t("SupportNotAvailable")}{" "}
        {moment(expiresDate).startOf("day").format("D MMMM, YYYY")}
        {". "}
        {t("LicenseRenewal")}
      </Text>
    </StyledHeader>
  ) : (
    <StyledHeader>
      <Text className="payments-header">{t("TrialPeriodExpired")}</Text>
      <Text className="payments-header-additional_support">
        {t("ThanksToUser")}
      </Text>
    </StyledHeader>
  );
};

HeaderContainer.propTypes = {
  culture: PropTypes.string,
  utcHoursOffset: PropTypes.number,
  expiresDate: PropTypes.object,
  trialMode: PropTypes.bool,
};
function mapStateToProps({ auth, payments }) {
  const { culture, utcHoursOffset } = auth.settings;
  const { expiresDate, trialMode } = payments.currentLicense;
  return {
    culture,
    utcHoursOffset,
    expiresDate,
    trialMode,
  };
}
export default connect(mapStateToProps)(withRouter(HeaderContainer));
