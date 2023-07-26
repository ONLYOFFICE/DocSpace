import CopyReactSvgUrl from "PUBLIC_DIR/images/copy.react.svg?url";
import React from "react";
import { observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import { Text, HelpButton, InputBlock } from "@docspace/components";
import toastr from "@docspace/components/toast/toastr";

import copy from "copy-to-clipboard";

const MetadataUrlField = ({
  labelText,
  name,
  placeholder,
  tooltipContent,
  tooltipClass,
}) => {
  const { t } = useTranslation("Translations");

  const onCopyClick = () => {
    copy(placeholder);
    toastr.success(t("Translations:LinkCopySuccess"));
  };

  return (
    <div className="metadata-field">
      <Text className="label" fontSize="13px" as="div" fontWeight={600}>
        {labelText}
        <HelpButton
          place="right"
          offsetRight={0}
          tooltipContent={tooltipContent}
          className={tooltipClass}
        />
      </Text>
      <InputBlock
        className="input"
        iconButtonClassName={name}
        isDisabled
        name={name}
        placeholder={placeholder}
        iconName={CopyReactSvgUrl}
        iconSize={16}
        onIconClick={onCopyClick}
      />
    </div>
  );
};

export default observer(MetadataUrlField);
