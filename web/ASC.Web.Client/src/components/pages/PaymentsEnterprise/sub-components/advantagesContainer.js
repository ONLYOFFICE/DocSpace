import React, { useEffect } from "react";

import styled from "styled-components";
import { utils } from "asc-web-common";
import { Text, utils as Utils, Box } from "asc-web-components";
import { createI18N } from "../../../../helpers/i18n";
import { useTranslation } from "react-i18next";
const { changeLanguage } = utils;
const { tablet } = Utils.device;

const i18n = createI18N({
  page: "PaymentsEnterprise",
  localesPath: "pages/PaymentsEnterprise",
});

const StyledBodyAdvantages = styled.div`
  display: grid;
  padding: 32px;
  grid-template-areas:
    "header header"
    "firstI first"
    "secondI second"
    "thirdI third"
    "fourI four"
    "fiveI five";

  .wrapper {
    display: flex;
    align-items: center;
  }

  grid-template-columns: 24px 1fr;
  grid-template-rows: min-content min-content;
  grid-auto-rows: min-content;
  grid-row-gap: 18px;
  grid-column-gap: 8px;
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
  }

  .second-advantages {
    grid-area: second;
    z-index: 1;
  }
  .third-advantages {
    grid-area: third;
    z-index: 1;
  }
  .four-advantages {
    grid-area: four;
    z-index: 1;
  }
  .five-advantages {
    grid-area: five;
    z-index: 1;
  }
  .header-advantages {
    grid-area: header;
    z-index: 1;
  }
  .first_icon {
    grid-area: firstI;
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
    .main_icon {
      display: none;
    }
  }

  @media (max-width: 632px) {
    grid-row-gap: 20px;
    grid-column-gap: 12px;
  }
`;

const AdvantagesContainer = () => {
  useEffect(() => {
    changeLanguage(i18n);
  }, []);

  const { t } = useTranslation("translation", { i18n });
  return (
    <StyledBodyAdvantages>
      <Text className="header-advantages">{t("SubscriptionGet")}</Text>
      <img
        className="first_icon"
        src="images/payments_enterprise_cubes.svg"
        width="24px"
        height="23px"
        alt="Icon_cubes"
      />
      <Box className="wrapper">
        <Text className="first-advantages  advantages">
          {t("OnlyOfficeEditor")}
        </Text>
      </Box>
      <img
        className="second_icon"
        src="images/payments_enterprise_lock.svg"
        width="24px"
        height="23px"
        alt="Icon_lock"
      />
      <Box className="wrapper">
        <Text className="second-advantages  advantages">
          {t("PrivateRooom")}
        </Text>
      </Box>
      <img
        className="third_icon"
        src="images/payments_enterprise_smartphone.svg"
        width="24px"
        height="23px"
        alt="Icon_smartphone"
      />
      <Box className="wrapper">
        <Text className="third-advantages  advantages">{t("WebEditors")}</Text>
      </Box>
      <img
        className="four_icon"
        src="images/payments_enterprise_update.svg"
        width="24px"
        height="23px"
        alt="Icon_update"
      />
      <Box className="wrapper">
        <Text className="four-advantages  advantages">
          {t("FunctionalityAndSecurity")}
        </Text>
      </Box>
      <img
        className="five_icon"
        src="images/payments_enterprise_help.svg"
        width="24px"
        height="23px"
        alt="Icon_help"
      />
      <Box className="wrapper">
        <Text className="five-advantages  advantages">
          {t("ProfessionalTechSupport")}
        </Text>
      </Box>
      <img
        className="payments-icon-enterprise main_icon"
        src="images/payments_enterprise.svg"
        width="222px"
        height="247px"
        alt="Icon_main"
      />
    </StyledBodyAdvantages>
  );
};

export default AdvantagesContainer;
