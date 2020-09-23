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

  grid-template-columns: 1fr;
  grid-template-rows: repeat(4, min-content);
  grid-row-gap: 18px;

  background: url("images/payments_enterprise.svg") #f8f9f9 bottom 32px right
    32px no-repeat;

  .header-advantages {
    line-height: 30px;
    padding-bottom: 15px;
  }

  .row-advantages {
    display: flex;
    .wrapper {
      align-items: center;
    }
  }

  @media ${tablet} {
    background: #f8f9f9;
  }
`;

const AdvantagesContainer = () => {
  useEffect(() => {
    changeLanguage(i18n);
  }, []);

  const { t } = useTranslation("translation", { i18n });
  return (
    <StyledBodyAdvantages>
      <Text className="header-advantages" fontSize="22px" isBold={true}>
        {t("SubscriptionGet")}
      </Text>

      <Box className="row-advantages">
        <img
          src="images/payments_enterprise_cubes.svg"
          width="24px"
          height="23px"
          alt="Icon_cubes"
        />
        <Box className="wrapper" marginProp="0 0 0 8px">
          <Text isBold={true}>{t("OnlyOfficeEditor")}</Text>
        </Box>
      </Box>

      <Box className="row-advantages">
        <img
          src="images/payments_enterprise_lock.svg"
          width="24px"
          height="23px"
          alt="Icon_lock"
        />
        <Box className="wrapper" marginProp="0 0 0 8px">
          <Text isBold={true}>{t("PrivateRooom")}</Text>
        </Box>
      </Box>

      <Box className="row-advantages">
        <img
          src="images/payments_enterprise_smartphone.svg"
          width="24px"
          height="23px"
          alt="Icon_smartphone"
        />
        <Box className="wrapper" marginProp="0 0 0 8px">
          <Text isBold={true}>{t("WebEditors")}</Text>
        </Box>
      </Box>

      <Box className="row-advantages">
        <img
          src="images/payments_enterprise_update.svg"
          width="24px"
          height="23px"
          alt="Icon_update"
        />
        <Box className="wrapper" marginProp="0 0 0 8px">
          <Text isBold={true}>{t("FunctionalityAndSecurity")}</Text>
        </Box>
      </Box>

      <Box className="row-advantages">
        <img
          src="images/payments_enterprise_help.svg"
          width="24px"
          height="23px"
          alt="Icon_help"
        />
        <Box className="wrapper" marginProp="0 0 0 8px">
          <Text isBold={true}>{t("ProfessionalTechSupport")}</Text>
        </Box>
      </Box>
    </StyledBodyAdvantages>
  );
};

export default AdvantagesContainer;
