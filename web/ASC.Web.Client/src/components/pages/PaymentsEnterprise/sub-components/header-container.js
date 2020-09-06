import React, { useEffect } from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import { connect } from "react-redux";
import { Router, Route, Switch, Redirect } from "react-router-dom";

import moment from "moment";
// import moment "moment/min/moment-with-locales";
import { Text, utils } from "asc-web-components";
import { history, store } from "asc-web-common";
const { tablet } = utils.device;

const StyledHeader = styled.div`
  position: static;
  .payments-header {
    margin-top: 46px;
    height: 32px;
    font-style: normal;
    font-weight: bold;
    font-size: 27px;
    line-height: 32px;
    color: #333333;
    margin-bottom: 8px;
  }
  .payments-header-additional_support {
  }
  .payments-header-additional_portals {
    margin-top: 13px;
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
    .payments-header-additional_support {
      font-weight: 600;
      line-height: 20px;
      color: #c96c27;
    }
    .payments-header-additional_portals {
      line-height: 20px;
      margin-top: 10px;
    }
  }

  @media (max-width: 632px) {
    width: 343px;
    .payments-header {
      margin-top: 0px;
      height: 96px;
    }
    .sd {
      height: 70px;
    }
    .payments-header-additional_support {
      width: 343px;
      line-height: 20px;
      color: #333333;
      font-style: normal;
      font-weight: normal;
    }
    .payments-header-additional_portals {
      margin-top: 10px;
    }
  }
`;

const { setCurrentError } = store.auth.actions;
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
  // this.props.error !== true && this.props.setCurrentError(true);

  return moment(
    moment.utc(expiresDate).set("hour", 0).set("minute", 0).set("second", 0)
  ).isAfter(
    currentUserDate.set("hour", 0).set("minute", 0).set("second", 0)
  ) ? (
    <StyledHeader>
      <Text className="payments-header">{t("Using")}</Text>
      <Text className="payments-header-additional_support">
        {t("SubscriptionAndUpdatesExpires")}
        {moment.utc(expiresDate).format("LL")}
        {/* Техническая поддержка и обновления недоступны для вашей лицензии с 1
          марта 2021 года. */}
      </Text>
      {/* <Text className="payments-header-additional_portals">
          {t("createdPortals")} {createPortals}
        </Text> */}
    </StyledHeader>
  ) : !trialMode ? (
    <StyledHeader>
      <Text className="payments-header">{t("Using")}</Text>
      <Text className="payments-header-additional_support">
        {t("SupportNotAvailable")}
        {moment.utc(expiresDate).startOf("day").format("dddd, MMMM D, YYYY")}
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
function mapStateToProps(state) {
  return {
    settings: state.auth.settings,
    modules: state.auth.modules,
  };
}
export default connect(mapStateToProps, { setCurrentError })(HeaderContainer);
