import React, { useEffect } from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import { Text, utils } from "asc-web-components";
import { WebStorageStateStore } from "oidc-client";

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

class HeaderContainer extends React.Component {
  constructor(props) {
    super(props);

    const { expiresDate, trialMode } = props;

    this.state = {
      expiresDate: expiresDate,
      trialMode: trialMode,
    };
  }

  componentDidUpdate(prevProps) {
    const { expiresDate, trialMode } = this.props;

    if (expiresDate !== prevProps.expiresDate) {
      this.setState({ expiresDate: expiresDate });
    }
  }
  render() {
    const { t, culture, utcHoursOffset, trialMode } = this.props;
    const { expiresDate } = this.state;

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

        <Text className="payments-header-additional_support" color="#C96C27">
          {t("SupportNotAvailable")}{" "}
          {moment(expiresDate).startOf("day").format("ddd, D MMMM, YYYY")}
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
  return {
    culture: auth.settings.culture,
    utcHoursOffset: auth.settings.utcHoursOffset,
    expiresDate: payments.currentLicense.expiresDate,
    trialMode: payments.currentLicense.trialMode,
  };
}
export default connect(mapStateToProps)(withRouter(HeaderContainer));
