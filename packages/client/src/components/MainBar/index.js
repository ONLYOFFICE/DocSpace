import React from "react";
import { inject, observer } from "mobx-react";
import styled, { css } from "styled-components";
import { isMobileOnly } from "react-device-detect";

import Bar from "./Bar";

const StyledContainer = styled.div`
  width: 100%;
  max-width: 100%;

  ${isMobileOnly &&
  css`
    width: calc(100% + 16px);
    max-width: calc(100% + 16px);

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
}) => {
  React.useEffect(() => {
    return () => setMaintenanceExist && setMaintenanceExist(false);
  }, []);

  const isVisibleBar =
    !isNotPaidPeriod &&
    pathname !== "/confirm/LinkInvite" &&
    pathname !== "/preparation-portal";

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
  } = settingsStore;
  const { isNotPaidPeriod } = currentTariffStatusStore;
  const { firstLoad } = filesStore;

  return {
    firstLoad,
    checkedMaintenance,
    snackbarExist,
    setMaintenanceExist,
    isNotPaidPeriod,
  };
})(observer(MainBar));
