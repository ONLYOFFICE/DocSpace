import React, { useEffect, useState } from "react";
import { inject, observer } from "mobx-react";
import styled from "styled-components";

import LoginSettings from "./login-settings";
import SocialNetworks from "./social-networks";
import ActiveSession from "./active-session";

const StyledWrapper = styled.div`
  width: 100%;
  display: flex;
  flex-direction: column;
  gap: 24px;
`;

const LoginContent = (props) => {
  const { setBackupCodes, tfaSettings, getBackupCodes } = props;
  const [backupCodesCount, setBackupCodesCount] = useState(0);
  const tfaOn = tfaSettings && tfaSettings !== "none";

  const fetchData = async () => {
    if (tfaOn) {
      const codes = await getBackupCodes();
      setBackupCodes(codes);

      let backupCodesCount = 0;
      if (codes && codes.length > 0) {
        codes.map((item) => {
          if (!item.isUsed) {
            backupCodesCount++;
          }
        });
      }
      setBackupCodesCount(backupCodesCount);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  return (
    <StyledWrapper>
      {tfaOn && <LoginSettings backupCodesCount={backupCodesCount} />}
      <SocialNetworks />
      <ActiveSession />
    </StyledWrapper>
  );
};

export default inject(({ auth }) => {
  const { tfaStore } = auth;
  const { getBackupCodes, tfaSettings, setBackupCodes } = tfaStore;
  return {
    getBackupCodes,
    tfaSettings,
    setBackupCodes,
  };
})(observer(LoginContent));
