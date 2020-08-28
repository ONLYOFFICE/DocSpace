import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";

import { Text, utils } from "asc-web-components";

const { tablet, mobile } = utils.device;

const StyledHeader = styled.div`
  position: static;
  .payments-header {
    margin-top: 50px;
    height: 32px;
    font-style: normal;
    font-weight: bold;
    font-size: 27px;
    line-height: 32px;
    color: #333333;
    margin-bottom: 8px;
  }
  .payments-header-additional_support {
    margin-top: 8px;
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
      margin-top: 12px;
    }
  }

  @media ${mobile} {
    width: 343px;
    .payments-header {
      margin-top: 0px;
      height: 96px;
    }
    .payments-header-additional_support {
      width: 343px;
      line-height: 20px;
      color: #333333;
      font-style: normal;
      font-weight: normal;
    }
  }
`;

const HeaderContainer = ({ t }) => {
  return (
    <StyledHeader>
      <Text className="payments-header">{t("Using")}</Text>
      <sd>
        <Text className="payments-header-additional_support">
          {t("SubscriptionAndUpdatesExpires")} 1 марта 2021 года.
        </Text>
        <Text className="payments-header-additional_portals">
          Порталы, созданные вами: 1/2
        </Text>
      </sd>
    </StyledHeader>
  );
};

HeaderContainer.propTypes = {
  t: PropTypes.func.isRequired,
};

export default HeaderContainer;
