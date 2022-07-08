import Button from "@appserver/components/button";
import ComboBox from "@appserver/components/combobox";
import Text from "@appserver/components/text";
import React from "react";
import { StyledBackup } from "../StyledBackup";

const DirectConnectionContainer = (props) => {
  const { selectedAccount, accounts, onConnect, onSelectAccount, t } = props;

  return (
    <StyledBackup>
      <div className="backup_connection">
        <ComboBox
          className="backup_third-party-combo"
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
      <Text className="backup_third-party-text" fontWeight={"600"}>
        {"Folder name:"}
      </Text>
    </StyledBackup>
  );
};

export default DirectConnectionContainer;
