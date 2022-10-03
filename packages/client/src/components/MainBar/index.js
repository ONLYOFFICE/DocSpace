import React from "react";
import { inject, observer } from "mobx-react";
import styled, { css } from "styled-components";
import { isMobileOnly } from "react-device-detect";

import Bar from "./Bar";

import SnackBar from "@docspace/components/snackbar";

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

//TODO: remove commented code after update bar logic

const MainBar = ({
  firstLoad,
  checkedMaintenance,
  snackbarExist,
  setMaintenanceExist,
}) => {
  // const [isVisible, setIsVisible] = React.useState(false);

  React.useEffect(() => {
    // setTimeout(() => setIsVisible(true), 9000);
    return () => setMaintenanceExist && setMaintenanceExist(false);
  }, []);

  return (
    <StyledContainer id={"main-bar"} className={"main-bar"}>
      {checkedMaintenance && !snackbarExist && (
        <Bar firstLoad={firstLoad} setMaintenanceExist={setMaintenanceExist} />
      )}
      {/* {isVisible && (
        <SnackBar
          headerText={"Rooms is about to be exceeded: 10 / 12"}
          text={
            "You can archived the unnecessary rooms or click here to find a better pricing plan for your portal."
          }
          isCampaigns={false}
          opacity={1}
          onLoad={() => console.log("load snackbar")}
        />
      )} */}
    </StyledContainer>
  );
};

export default inject(({ auth, filesStore }) => {
  const {
    checkedMaintenance,
    setMaintenanceExist,
    snackbarExist,
  } = auth.settingsStore;

  const { firstLoad } = filesStore;

  return { firstLoad, checkedMaintenance, snackbarExist, setMaintenanceExist };
})(observer(MainBar));
