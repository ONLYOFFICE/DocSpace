import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";

import { Text, utils } from "asc-web-components";
const { tablet } = utils.device;
const StyledBodyAdvantages = styled.div`
  margin-top: 42px;

  display: flex;
  flex-direction: column;
  padding: 32px 32px 32px 0px;
  width: 888px;
  height: 247px;
  position: static;

  background: #f8f9f9;
  border-radius: 6px 6px 0px 0px;

  .payments-icon-enterprise {
    position: absolute;
    margin-left: 666px;
    z-index: 0;
  }
  /*
  .first_icon {
    margin: 64px 0px 193px 32px;
  } */
  /* .second_icon {
    margin: 104px 0px 193px 32px;
  } */
  /* .third_icon {
    margin: 144px 0px 193px 32px;
  }
  .four_icon {
    margin: 184px 0px 193px 32px;
  }
  .five_icon {
    margin: 224px 0px 193px 32px;
  } */
  /* .payments-body {
    /* margin-right: 609px; 
    margin-bottom: 22px;
    margin-left: 64px;

    font-weight: bold;
    font-size: 13px;
    color: #555f65;
  } */
  /* .first-advantages {
    margin-top: 37px;
    /* width: 247px; 
  } */
  .second-advantages {
    /* width: 278px; */
  }
  .third-advantages {
    /* width: 124px; */
  }
  .four-advantages {
    /* width: 366px; */
  }
  .five-advantages {
    /* width: 276px; */
  }
  .advantages-enterprise {
    width: 856px;
    height: 30px;
    font-weight: bold;
    font-size: 22px;
    line-height: 30px;
    margin: 0px 32px 0 32px;
  }
  .payments-body {
    font-weight: bold;
    font-size: 13px;
    line-height: 16px;
  }

  .styled-advantages {
    margin-top: 34px;
    margin-left: 32px;
    display: flex;
  }
  .styled-first-advantages {
    margin-top: 34px;
    margin-left: 32px;
    display: flex;
  }
  .styled-second-advantages {
    margin-top: 18px;
    margin-left: 32px;
    display: flex;
  }

  .styled-third-advantages {
    margin-top: 17px;
    margin-left: 32px;
    display: flex;
  }

  .styled-four-advantages {
    margin-top: 17px;
    margin-left: 32px;
    display: flex;
    z-index: 1;
  }

  .styled-five-advantages {
    margin-top: 17px;
    margin-left: 32px;
    display: flex;
  }

  .advantages {
    margin-left: 8px;
    margin-top: 4px;
  }
  /* .second-advantages {
    margin-left: 8px;
    margin-top: 4px;
  }
  .first-advantages {
    margin-left: 8px;
    margin-top: 4px;
  }
  .third-advantages {
    margin-left: 8px;
    margin-top: 4px;
  }
  .four-advantages {
    margin-left: 8px;
    margin-top: 4px;
  }
  .five-advantages {
    margin-left: 8px;
    margin-top: 4px;
  } */
  @media ${tablet} {
    width: 568px;
    height: 249px;
    margin-top: 41px;
    .main_icon {
      display: none;
    }
    .styled-first-advantages {
      margin-top: 33px;
      margin-left: 32px;
      display: flex;
    }
    .styled-second-advantages {
      margin-top: 17px;
      margin-left: 32px;
      display: flex;
    }

    .styled-third-advantages {
      margin-top: 16px;
      margin-left: 32px;
      display: flex;
    }

    .styled-four-advantages {
      margin-top: 16px;
      margin-left: 32px;
      display: flex;
      z-index: 1;
    }

    .styled-five-advantages {
      margin-top: 17px;
      margin-left: 32px;
      display: flex;
    }
  }
  @media (max-width: 632px) {
    margin-top: 16px;
    width: 311px;
    height: 301px;
    .styled-first-advantages {
      margin-top: 33px;
      margin-left: 32px;
      display: flex;
    }
    .styled-second-advantages {
      margin-top: 17px;
      margin-left: 32px;
      display: flex;
    }

    .styled-third-advantages {
      margin-top: 16px;
      margin-left: 32px;
      display: flex;
    }

    .styled-four-advantages {
      margin-top: 16px;
      margin-left: 32px;
      display: flex;
      z-index: 1;
    }

    .styled-five-advantages {
      margin-top: 17px;
      margin-left: 32px;
      display: flex;
    }
  }
`;

const StyledFirstAdvantages = styled.div``;
const StyledSecondAdvantages = styled.div``;
const AdvantagesContainer = ({ t }) => {
  return (
    <StyledBodyAdvantages>
      <Text className="advantages-enterprise">{t("SubscriptionGet")}</Text>
      <div className="styled-first-advantages">
        <img
          className="first_icon"
          src="images/payments_enterprise_cubes.svg"
          width="24px"
          height="23px"
          alt="Icon"
        />
        <Text className="payments-body advantages">
          {t("OnlyOfficeEditor")}
        </Text>
      </div>
      <div className="styled-second-advantages">
        <img
          className="second_icon"
          src="images/payments_enterprise_lock.svg"
          width="24px"
          height="23px"
          alt="Icon"
        />
        <Text className="payments-body advantages">{t("PrivateRooom")}</Text>
      </div>
      <div className="styled-third-advantages">
        <img
          className="third_icon"
          src="images/payments_enterprise_smartphone.svg"
          width="24px"
          height="23px"
          alt="Icon"
        />
        <Text className="payments-body advantages">{t("WebEditors")}</Text>
      </div>
      <div className="styled-four-advantages">
        <img
          className="four_icon"
          src="images/payments_enterprise_update.svg"
          width="24px"
          height="23px"
          alt="Icon"
        />
        <Text className="payments-body advantages">
          {t("FunctionalityAndSecurity")}
        </Text>
      </div>
      <div className="styled-five-advantages">
        <img
          className="five_icon"
          src="images/payments_enterprise_help.svg"
          width="24px"
          height="23px"
          alt="Icon"
        />
        <Text className="payments-body advantages">
          {t("ProfessionalTechSupport")}
        </Text>
      </div>

      <img
        className="payments-icon-enterprise main_icon"
        src="images/payments_enterprise.svg"
        width="222px"
        height="247px"
        alt="Icon"
      />
    </StyledBodyAdvantages>
  );
};

AdvantagesContainer.propTypes = {
  t: PropTypes.func.isRequired,
};

export default AdvantagesContainer;
