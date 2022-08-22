import React, { useEffect, useState } from "react";
import styled from "styled-components";
import { withRouter } from "react-router";
import { useTranslation, Trans } from "react-i18next";
import PropTypes from "prop-types";
import Section from "@docspace/common/components/Section";
import Loaders from "@docspace/common/components/Loaders";
import { setDocumentTitle } from "@docspace/client/src/helpers/filesUtils";
import { inject, observer } from "mobx-react";
import Text from "@docspace/components/text";
import CurrentTariffContainer from "./CurrentTariffContainer";
import PriceCalculation from "./PriceCalculation";
import BenefitsContainer from "./BenefitsContainer";
import { smallTablet } from "@docspace/components/utils/device";
import ContactContainer from "./ContactContainer";
import toastr from "@docspace/components/toast/toastr";
import moment from "moment";
import { HelpButton } from "@docspace/components";
import PayerInformationContainer from "./PayerInformationContainer";

const StyledBody = styled.div`
  max-width: 660px;

  .payment-info_suggestion {
    margin-bottom: 12px;
  }

  .payment-info {
    display: grid;
    grid-template-columns: repeat(2, minmax(100px, 320px));
    grid-gap: 20px;
    margin-bottom: 20px;
    @media ${smallTablet} {
      grid-template-columns: 1fr;
      grid-template-rows: 1fr 1fr;
    }
  }
  .payment-info_wrapper {
    display: flex;
    margin-bottom: 20px;
    margin-top: 12px;
    div {
      margin: auto 0;
    }
    .payment-info_managers-price {
      margin-right: 8px;
    }
  }
`;

let dueDate;
const PaymentsPage = ({
  setQuota,
  isStartup,
  getPaymentPrices,
  pricePerManager,
  setPortalQuota,
  setPortalTariff,
  language,
  portalTariff,
  portalQuota,
}) => {
  const { t, ready } = useTranslation(["Payments", "Settings"]);

  const [isInitialLoading, setIsInitialLoading] = useState(true);

  useEffect(() => {
    setDocumentTitle(t("Settings:Payments"));
  }, [ready]);

  useEffect(() => {
    (async () => {
      moment.locale(language);

      const requests = [];

      requests.push(setPortalTariff(), setQuota());

      if (Object.keys(portalQuota).length === 0) setPortalQuota();
      if (!pricePerManager) getPaymentPrices();

      try {
        await Promise.all(requests);

        if (portalTariff) dueDate = moment(portalTariff.dueDate).format("LL");
      } catch (error) {
        toastr.error(error);
      }

      setIsInitialLoading(false);
    })();
  }, []);

  const renderTooltip = () => {
    return (
      <>
        <HelpButton
          iconName={"/static/images/help.react.svg"}
          tooltipContent={
            <>
              <Text isBold>{t("ManagerTypesDescription")}</Text>
              <br />
              <Text isBold>{t("Administrator")}</Text>
              <Text>{t("AdministratorDescription")}</Text>
              <br />
              <Text isBold>{t("RoomManager")}</Text>
              <Text>{t("RoomManagerDescription")}</Text>
            </>
          }
        />
      </>
    );
  };

  return isInitialLoading ? (
    <Loaders.PaymentsLoader />
  ) : (
    <StyledBody>
      <Text noSelect fontSize="16px" isBold>
        {isStartup ? t("StartupTitle") : t("BusinessTitle")}
      </Text>
      <PayerInformationContainer />
      <CurrentTariffContainer />
      <Text noSelect fontSize="16px" isBold className="payment-info_suggestion">
        {isStartup ? t("StartupSuggestion") : t("BusinessSuggestion")}
      </Text>

      {!isStartup && (
        <Text noSelect fontSize={"14"} className="payment-info_managers-price">
          <Trans t={t} i18nKey="BusinessFinalDateInfo" ns="Payments">
            {{ finalDate: dueDate }}
          </Trans>
        </Text>
      )}

      <div className="payment-info_wrapper">
        <Text
          noSelect
          fontWeight={600}
          fontSize={"14"}
          className="payment-info_managers-price"
        >
          <Trans t={t} i18nKey="StartPrice" ns="Payments">
            {{ price: pricePerManager }}
          </Trans>
        </Text>

        {renderTooltip()}
      </div>

      <div className="payment-info">
        <PriceCalculation t={t} />
        <BenefitsContainer t={t} />
      </div>

      <ContactContainer t={t} />
    </StyledBody>
  );
};

PaymentsPage.propTypes = {
  isLoaded: PropTypes.bool,
};

export default inject(({ auth, payments }) => {
  const {
    setQuota,
    setPortalQuota,
    setPortalTariff,
    language,
    portalTariff,
    getPaymentPrices,
    pricePerManager,
    portalQuota,
  } = auth;

  const { organizationName } = auth.settingsStore;
  const { setTariffsInfo, tariffsInfo } = payments;

  const isStartup = false;

  return {
    setQuota,

    setPortalQuota,
    setPortalTariff,
    portalTariff,
    getPaymentPrices,
    language,
    organizationName,
    setTariffsInfo,
    tariffsInfo,
    isStartup,
    pricePerManager,
    portalQuota,
  };
})(withRouter(observer(PaymentsPage)));
