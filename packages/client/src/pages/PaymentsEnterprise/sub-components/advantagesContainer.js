import React from "react";
import styled from "styled-components";

import Text from "@docspace/components/text";
import Box from "@docspace/components/box";
import { tablet } from "@docspace/components/utils/device";
import { useTranslation, Trans } from "react-i18next";
import { inject } from "mobx-react";
import { Base } from "@docspace/components/themes";

const StyledBodyAdvantages = styled.div`
  display: grid;
  padding: 32px;

  grid-template-columns: 1fr;
  grid-template-rows: repeat(4, min-content);
  grid-row-gap: 18px;

  background: url("images/payments_enterprise.svg")
    ${(props) => props.theme.client.paymentsEnterprise.background} bottom 32px
    right 32px no-repeat;

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
    background: ${(props) => props.theme.client.paymentsEnterprise.background};
  }
`;

StyledBodyAdvantages.defaultProps = { theme: Base };

const AdvantagesContainer = ({ organizationName }) => {
  const { t } = useTranslation("PaymentsEnterprise");
  return (
    <StyledBodyAdvantages>
      <Text className="header-advantages" fontSize="22px" isBold={true}>
        {t("SubscriptionRenewedLicense")}
      </Text>

      <Box className="row-advantages">
        <img
          src="images/payments_enterprise_cubes.svg"
          width="24px"
          height="23px"
          alt="Icon_cubes"
        />
        <Box className="wrapper" marginProp="0 0 0 8px">
          <Text isBold={true}>
            <Trans t={t} i18nKey="AdvantageEditor" ns="PaymentsEnterprise">
              {{ organizationName }}
            </Trans>
          </Text>
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
          <Text isBold={true}>{t("AdvantagePrivateRooom")}</Text>
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
          <Text isBold={true}>{t("AdvantageWebEditors")}</Text>
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
          <Text isBold={true}>{t("AdvantageUpdates")}</Text>
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
          <Text isBold={true}>{t("AdvantageProfessionalTechSupport")}</Text>
        </Box>
      </Box>
    </StyledBodyAdvantages>
  );
};

export default inject(({ auth }) => ({
  organizationName: auth.settingsStore.organizationName,
}))(AdvantagesContainer);
