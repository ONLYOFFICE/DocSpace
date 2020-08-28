import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";

import { Text, utils } from "asc-web-components";

const { tablet, mobile } = utils.device;

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

  @media ${mobile} {
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

const HeaderContainer = ({ t, dateExpires, createPortals }) => {
  return (
    <StyledHeader>
      <Text className="payments-header">{t("Using")}</Text>
      <sd>
        <Text className="payments-header-additional_support">
          {t("SubscriptionAndUpdatesExpires")} {dateExpires}
          {/* Техническая поддержка и обновления недоступны для вашей лицензии с 1
          марта 2021 года. */}
        </Text>
        <Text className="payments-header-additional_portals">
          {t("createdPortals")} {createPortals}
        </Text>
      </sd>
    </StyledHeader>
  );
};

HeaderContainer.propTypes = {
  dateExpires: PropTypes.string.isRequired,
  createPortals: PropTypes.string.isRequired,
  t: PropTypes.func.isRequired,
};

export default HeaderContainer;
