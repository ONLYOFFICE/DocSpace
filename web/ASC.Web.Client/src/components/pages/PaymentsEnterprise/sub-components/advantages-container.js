import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";

import { Text, utils } from "asc-web-components";
const { tablet } = utils.device;
const StyledBodyAdvantages = styled.div`
  display: grid;
  padding-left: 32px;
  padding-top: 32px;
  padding-right: 8px;
  width: 880px;
  height: 279px;
  grid-template-areas:
    "header header"
    "firstI first"
    "secondI second"
    "thirdI third"
    "fourI four"
    "fiveI five";

  grid-template-columns: 2.7% 98.3%;
  grid-template-rows: 11.6% 2.9% 2.8% 2.8% 2.8% 1.1%;
  grid-row-gap: 32px;
  grid-column-gap: 5px;

  margin: 0;
  background: #f8f9f9;
  .advantages {
    font-weight: bold;
    font-size: 13px;
    line-height: 16px;
  }
  .header-advantages {
    font-weight: bold;
    font-size: 22px;
    line-height: 30px;
  }
  .main_icon {
    position: absolute;
    margin-left: 634px;
    z-index: 0;
  }
  .first-advantages {
    grid-area: first;
    z-index: 1;
    /* padding: 3px; */
  }
  .first_icon {
    grid-area: firstI;
  }
  .second-advantages {
    grid-area: second;
    padding: 3px;
    z-index: 1;
  }
  .third-advantages {
    grid-area: third;
    padding: 3px;
    z-index: 1;
  }
  .four-advantages {
    grid-area: four;
    padding: 3px;
    z-index: 1;
  }
  .five-advantages {
    grid-area: five;
    padding: 3px;
    z-index: 1;
  }
  .header-advantages {
    grid-area: header;
    z-index: 1;
  }
  .second_icon {
    grid-area: secondI;
  }
  .third_icon {
    grid-area: thirdI;
  }
  .four_icon {
    grid-area: fourI;
  }
  .five_icon {
    grid-area: fiveI;
  }
  @media ${tablet} {
    width: 560px;
    height: 100%;
    margin-right: 32px;
    grid-template-columns: 2.7% 90.3%;
    grid-template-rows: min-content min-content;
    grid-auto-rows: min-content;
    grid-row-gap: 12px;
    grid-column-gap: 5px;
    .main_icon {
      display: none;
    }
    .header-advantages {
      margin-bottom: 21px;
    }
    .first-advantages {
      padding-left: 11px;
      margin-bottom: 6px;
      padding-top: 3px;
    }
    .four-advantages {
      padding-left: 5px;
    }
    .second-advantages {
      padding-left: 11px;
      margin-bottom: 5px;
      padding-top: 3px;
    }
    .third-advantages {
      padding-left: 11px;
      margin-bottom: 5px;
      padding-top: 3px;
    }
    .four-advantages {
      padding-left: 11px;
      padding-top: 3px;
      margin-bottom: 6px;
    }
    .five-advantages {
      padding-left: 11px;
      padding-top: 3px;
      margin-bottom: 32px;
    }
  }

  @media (max-width: 632px) {
    padding-right: 32px;
    width: 280px;
    height: 100%;

    grid-template-columns: 7.9% 92.1%;
    /* grid-template-rows: 21% 2.8% 5.3% 1.5% 5.1% 40.1%; */
    grid-template-rows: min-content auto;
    grid-auto-rows: min-content;

    /* grid-template-columns: min-content auto;
    grid-auto-columns: min-content;  */
    /* grid-template-columns: 7.9% 92.1%; */

    /* grid-column-gap: 5px; */
    grid-row-gap: 13px;
    .header-advantages {
      margin-bottom: 21px;
    }
    .first-advantages {
      padding-left: 5px;
      margin-bottom: 4px;
      /* padding-top: 0; */
    }
    .four-advantages {
      padding-left: 5px;
    }
    .second-advantages {
      padding-left: 5px;
      margin-bottom: 2px;
      padding-top: 0px;
    }
    .third-advantages {
      padding-left: 5px;
      margin-bottom: 4px;
      padding-right: 0px;
    }
    .four-advantages {
      padding-left: 5px;
      padding-top: 0px;
    }
    .five-advantages {
      padding-left: 5px;
      padding-top: 0px;
      margin-bottom: 29px;
    }
  }
`;

const AdvantagesContainer = ({ t }) => {
  return (
    <StyledBodyAdvantages>
      <Text className="header-advantages">{t("SubscriptionGet")}</Text>

      <img
        className="first_icon"
        src="images/payments_enterprise_cubes.svg"
        width="24px"
        height="23px"
        alt="Icon"
      />
      <Text className="first-advantages  advantages">
        {t("OnlyOfficeEditor")}
      </Text>

      <img
        className="second_icon"
        src="images/payments_enterprise_lock.svg"
        width="24px"
        height="23px"
        alt="Icon"
      />
      <Text className="second-advantages  advantages">{t("PrivateRooom")}</Text>

      <img
        className="third_icon"
        src="images/payments_enterprise_smartphone.svg"
        width="24px"
        height="23px"
        alt="Icon"
      />
      <Text className="third-advantages  advantages">{t("WebEditors")}</Text>

      <img
        className="four_icon"
        src="images/payments_enterprise_update.svg"
        width="24px"
        height="23px"
        alt="Icon"
      />
      <Text className="four-advantages  advantages">
        {t("FunctionalityAndSecurity")}
      </Text>

      <img
        className="five_icon"
        src="images/payments_enterprise_help.svg"
        width="24px"
        height="23px"
        alt="Icon"
      />
      <Text className="five-advantages  advantages">
        {t("ProfessionalTechSupport")}
      </Text>

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
