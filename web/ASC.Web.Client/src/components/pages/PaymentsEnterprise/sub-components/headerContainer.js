import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import { connect } from "react-redux";
import { withRouter } from "react-router";
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
    font-size: 13px;
    line-height: 20px;
  }

  @media ${tablet} {
    width: 600px;
    .payments-header {
      margin-top: 39px;
      font-style: normal;
      font-weight: bold;
      font-size: 27px;
      line-height: 32px;
    }
  }

  @media (max-width: 632px) {
    width: 343px;
    .payments-header {
      margin-top: 1px;
    }
    .payments-header-additional_support {
      margin-bottom: 16px;
    }
  }
`;

class HeaderContainer extends React.Component {
  render() {
    const { t, culture, utcHoursOffset, trialMode, expiresDate } = this.props;

    const moment = require("moment");
    require("moment/min/locales.min");
    moment.locale(culture);
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
  }
}

HeaderContainer.propTypes = {
  t: PropTypes.func.isRequired,
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
