import React, { useState, useEffect } from "react";
import SelectFileInput from "client/SelectFolderInput";
import { inject, observer } from "mobx-react";

let accounts = [];
const ThirdPartyResources = (props) => {
  const { t, isDocSpace } = props;

  useEffect(() => {
    accounts = [
      {
        key: "0",
        label: "Google Drive",
      },
      {
        key: "1",
        label: "OneDrive ",
      },
      {
        key: "2",
        label: "Dropbox ",
      },
      {
        key: "3",
        label: "Box.com",
      },
    ];
    setSelectedAccount(accounts[0]);
  }, []);

  const [isConnected, setIsConnected] = useState(false);

  const [selectedAccount, setSelectedAccount] = useState();

  const onConnect = () => {
    setIsConnected(!isConnected);
  };

  const onSelectAccount = (options) => {
    const key = options.key;
    const label = options.label;
    setSelectedAccount({ key, label });
  };

  return !isDocSpace ? (
    <SelectFileInput
      {...props}
      foldersType="third-party"
      searchParam=".gz"
      filesListTitle={t("SelectFileInGZFormat")}
      withoutResetFolderTree
      ignoreSelectedFolderTree
      isArchiveOnly
      maxFolderInputWidth="446px"
    />
  ) : (
    <div className={"restore-backup_third-party-module"}>
      {/* <DirectConnectionContainer
        accounts={accounts}
        selectedAccount={selectedAccount}
        onSelectAccount={onSelectAccount}
        onConnect={onConnect}
        t={t}
      /> */}

      <SelectFileInput
        {...props}
        isDisabled={!isConnected}
        foldersType="third-party"
        searchParam=".gz"
        filesListTitle={t("SelectFileInGZFormat")}
        withoutResetFolderTree
        ignoreSelectedFolderTree
        isArchiveOnly
        maxFolderInputWidth="446px"
      />
    </div>
  );
};

export default inject(({ backup }) => {
  const { commonThirdPartyList } = backup;

  const isDocSpace = false;

  return {
    commonThirdPartyList,
    isDocSpace,
  };
})(observer(ThirdPartyResources));
