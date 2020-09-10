import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";

import { Text, utils } from "asc-web-components";

const { tablet } = utils.device;

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
    font-weight: normal;
    font-size: 13px;
    line-height: 20px;
  }

  @media ${tablet} {
    width: 600px;
    .payments-header {
      height: 64px;
      font-style: normal;
      font-weight: bold;
      font-size: 27px;
      line-height: 32px;
    }
  }

  @media (max-width: 632px) {
    width: 343px;
    .payments-header {
      margin-top: 0px;
      height: 96px;
    }

    .payments-header-additional_support {
      margin-bottom: 16px;
    }
  }
`;

const HeaderContainer = ({
  t,
  expiresDate,
  culture,
  utcHoursOffset,
  trialMode,
}) => {
  const moment = require("moment");
  require("moment/min/locales.min");
  moment.locale(culture);
  const currentUserDate = moment().utcOffset(utcHoursOffset);

  return moment(
    currentUserDate.set("hour", 0).set("minute", 0).set("second", 0)
  ).isAfter(
    moment.utc(expiresDate).set("hour", 0).set("minute", 0).set("second", 0)
  ) ? (
    <StyledHeader>
      <Text className="payments-header">{t("Using")}</Text>
      <Text className="payments-header-additional_support">
        {t("SubscriptionAndUpdatesExpires")}
        {moment.utc(expiresDate).format("LL")}
      </Text>
    </StyledHeader>
  ) : !trialMode ? (
    <StyledHeader>
      <Text className="payments-header">{t("Using")}</Text>

      <Text className="payments-header-additional_support">
        {t("SupportNotAvailable")}{" "}
        {moment.utc(expiresDate).startOf("day").format("dddd, MMMM D, YYYY")}
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
  dateExpires: PropTypes.string.isRequired,
  createPortals: PropTypes.string.isRequired,
  t: PropTypes.func.isRequired,
};

export default HeaderContainer;
