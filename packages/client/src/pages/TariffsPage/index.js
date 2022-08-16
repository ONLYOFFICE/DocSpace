import React, { useEffect } from "react";
import styled from "styled-components";
import { withRouter } from "react-router";
import { useTranslation, Trans } from "react-i18next";
import PropTypes from "prop-types";
import Section from "@docspace/common/components/Section";
import Loader from "@docspace/components/loader";
import { setDocumentTitle } from "@docspace/client/src/helpers/filesUtils";
import { inject, observer } from "mobx-react";
import Text from "@docspace/components/text";
import CurrentTariffContainer from "./CurrentTariffContainer";
import PriceCalculation from "./PriceCalculation";
import BenefitsContainer from "./BenefitsContainer";
import { smallTablet } from "@docspace/components/utils/device";
import ContactContainer from "./ContactContainer";
import toastr from "@docspace/components/toast/toastr";

const StyledBody = styled.div`
  max-width: 660px;
  p {
    margin-bottom: 16px;
  }
  .payments-info_suggestion {
    margin-bottom: 12px;
  }
  .payments-info_managers-price {
    margin-bottom: 20px;
  }
  .payments-info {
    display: grid;
    grid-template-columns: repeat(2, minmax(100px, 320px));
    grid-gap: 20px;
    margin-bottom: 20px;
    @media ${smallTablet} {
      grid-template-columns: 1fr;
      grid-template-rows: 1fr 1fr;
    }
  }
`;
const TariffsPageWrapper = ({
  setQuota,
  quota,
  setTariffsInfo,
  tariffsInfo,
  organizationName,
  isStartup,
  price,
  finalDate,
}) => {
  const { t, ready } = useTranslation("Payments");

  useEffect(() => {
    setDocumentTitle(t("TariffsPlans")); //TODO: need to specify
  }, [ready]);

  useEffect(() => {
    (async () => {
      try {
        await Promise.all([setQuota()]);
      } catch (error) {
        toastr.error(error);
      }
    })();
  }, []);

  return (
    <StyledBody>
      <Text noSelect fontSize="16px" isBold>
        {isStartup ? t("StartupTitle") : t("BusinessTitle")}
      </Text>
      <CurrentTariffContainer />
      <Text
        noSelect
        fontSize="16px"
        isBold
        className="payments-info_suggestion"
      >
        {isStartup ? t("StartupSuggestion") : t("BusinessSuggestion")}
      </Text>

      {!isStartup && (
        <Text noSelect fontSize={"14"} className="payments-info_managers-price">
          <Trans t={t} i18nKey="BusinessFinalDateInfo" ns="Payments">
            {{ finalDate: finalDate }}
          </Trans>
        </Text>
      )}

      <Text
        noSelect
        fontWeight={600}
        fontSize={"14"}
        className="payments-info_managers-price"
      >
        <Trans t={t} i18nKey="StartPrice" ns="Payments">
          {{ price: price }}
        </Trans>
      </Text>

      <div className="payments-info">
        <PriceCalculation t={t} price={price} />
        <BenefitsContainer t={t} />
      </div>

      <ContactContainer t={t} />
    </StyledBody>
  );
};

TariffsPageWrapper.propTypes = {
  isLoaded: PropTypes.bool,
};

const TariffsPage = (props) => {
  return (
    <Section>
      <Section.SectionBody>
        <TariffsPageWrapper {...props} />
      </Section.SectionBody>
    </Section>
  );
};
export default inject(({ auth, payments }) => {
  const { setQuota, quota } = auth;
  const { organizationName } = auth.settingsStore;
  const { setTariffsInfo, tariffsInfo } = payments;

  const isStartup = false;
  const price = "30";
  const finalDate = "17 February 2022";

  return {
    setQuota,
    quota,
    organizationName,
    setTariffsInfo,
    tariffsInfo,
    isStartup,
    price,
    finalDate,
  };
})(withRouter(observer(TariffsPage)));
