import React from "react";
import { inject, observer } from "mobx-react";
import Button from "@docspace/components/button";
import TextInput from "@docspace/components/text-input";
import Text from "@docspace/components/text";
import { ConfigurationWrapper } from "../StyledSpaces";
import { parseAddress } from "@docspace/components/utils/email";
import { useStore } from "SRC_DIR/store";

const ConfigurationSection = ({ t }) => {
  const [domain, setDomain] = React.useState<string>("");
  const [name, setName] = React.useState<string>("");

  const { spacesStore, authStore } = useStore();

  const { validatePortalName, setPortalSettings } = spacesStore;

  const onHandleClick = () => {
    setPortalSettings(domain, name);
    authStore.settingsStore.getAllPortals();
  };

  const onHandleDomain = (e: React.ChangeEvent<HTMLInputElement>) =>
    setDomain(e.target.value);

  const onHandleName = (e: React.ChangeEvent<HTMLInputElement>) =>
    setName(e.target.value);

  let parsed = parseAddress("test@" + domain);
  const isDomainError = domain.length > 0 && !parsed.isValid();
  const isNameError = name.length > 0 && (name.length > 100 || name.length < 6);

  return (
    <ConfigurationWrapper>
      <div className="spaces-configuration-header">
        <div className="spaces-configuration-title">
          <Text fontSize="16px" fontWeight={700}>
            {t("ConfigurationHeader")}
          </Text>
        </div>
        <Text>{t("ConfigurationDescription")}</Text>
      </div>
      <div className="spaces-input-wrapper">
        <div className="spaces-input-block">
          <div className="spaces-text-wrapper">
            <Text
              fontSize="13px"
              fontWeight="600"
              className="spaces-domain-text"
            >
              {t("Domain")}
            </Text>
            <Text color="#A3A9AE">(example.com)</Text>
          </div>

          <TextInput
            hasError={isDomainError}
            onChange={onHandleDomain}
            value={domain}
            placeholder={t("EnterDomain")}
            className="spaces-input"
          />
        </div>
        <div className="spaces-input-block">
          <Text fontSize="13px" fontWeight="600">
            {t("DocSpaceName")}
          </Text>
          <TextInput
            hasError={isNameError}
            onChange={onHandleName}
            value={name}
            placeholder={t("Common:EnterName")}
            className="spaces-input"
          />
        </div>
      </div>

      <Button
        isDisabled={!(name && domain) || isDomainError || isNameError}
        size="normal"
        className="spaces-button"
        label={t("Common:Connect")}
        onClick={onHandleClick}
        primary={true}
      />
    </ConfigurationWrapper>
  );
};

export default observer(ConfigurationSection);
