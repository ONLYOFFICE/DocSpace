import React, { useState, useEffect } from "react";
import SelectFileInput from "files/SelectFileInput";
import { inject, observer } from "mobx-react";
import ComboBox from "@appserver/components/combobox";
import Button from "@appserver/components/button";
import Text from "@appserver/components/text";

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
      <div className="restore-backup_connection">
        <ComboBox
          className="restore-backup_third-party-combo"
          options={accounts}
          selectedOption={{
            key: 0,
            label: selectedAccount?.label,
          }}
          onSelect={onSelectAccount}
          noBorder={false}
          scaledOptions
          dropDownMaxHeight={300}
          tabIndex={1}
        />

        <Button
          label={t("Common:Connect")}
          onClick={onConnect}
          size={"small"}
        />
      </div>
      <Text fontWeight={"600"}>{"Folder name:"}</Text>

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

  const isDocSpace = true;

  return {
    commonThirdPartyList,
    isDocSpace,
  };
})(observer(ThirdPartyResources));
