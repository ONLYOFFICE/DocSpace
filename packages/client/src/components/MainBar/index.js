import React from "react";
import { inject, observer } from "mobx-react";
import styled, { css } from "styled-components";
import { isMobileOnly } from "react-device-detect";
import { hugeMobile, mobile } from "@docspace/components/utils/device";
import Bar from "./Bar";

const StyledContainer = styled.div`
  width: 100%;
  max-width: 100%;

  ${isMobileOnly &&
  css`
    @media ${hugeMobile} {
      width: calc(100% + 16px);
      max-width: calc(100% + 16px);
    }

    @media ${mobile} {
      width: calc(100% + 8px);
      max-width: calc(100% + 8px);
    }

    margin-right: -16px;
    margin-top: 48px;
  `}

  #bar-banner {
    margin-bottom: -3px;
  }

  #bar-frame {
    min-width: 100%;
    max-width: 100%;
  }
`;

const pathname = window.location.pathname;
const MainBar = ({
  firstLoad,
  checkedMaintenance,
  snackbarExist,
  setMaintenanceExist,
  isNotPaidPeriod,
  isFrame,
}) => {
  React.useEffect(() => {
    return () => setMaintenanceExist && setMaintenanceExist(false);
  }, []);

  const isVisibleBar =
    !isNotPaidPeriod &&
    pathname.indexOf("confirm") === -1 &&
    pathname !== "/preparation-portal" &&
    !isFrame;

  return (
    <StyledContainer id={"main-bar"} className={"main-bar"}>
      {isVisibleBar && checkedMaintenance && !snackbarExist && (
        <Bar firstLoad={firstLoad} setMaintenanceExist={setMaintenanceExist} />
      )}
    </StyledContainer>
  );
};

export default inject(({ auth, filesStore }) => {
  const { currentTariffStatusStore, settingsStore } = auth;
  const {
    checkedMaintenance,
    setMaintenanceExist,
    snackbarExist,
    isFrame,
  } = settingsStore;
  const { isNotPaidPeriod } = currentTariffStatusStore;
  const { firstLoad } = filesStore;

  return {
    firstLoad,
    checkedMaintenance,
    snackbarExist,
    setMaintenanceExist,
    isNotPaidPeriod,
    isFrame,
  };
})(observer(MainBar));
