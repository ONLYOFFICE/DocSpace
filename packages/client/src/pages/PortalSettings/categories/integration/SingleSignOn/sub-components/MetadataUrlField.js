import CopyReactSvgUrl from "PUBLIC_DIR/images/copy.react.svg?url";
import React from "react";
import styled from "styled-components";
import { observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import { Text, HelpButton, InputBlock } from "@docspace/components";
import toastr from "@docspace/components/toast/toastr";

import copy from "copy-to-clipboard";

const StyledWrapper = styled.div`
  display: flex;
  flex-direction: column;
  gap: 4px;
  margin-bottom: 16px;
  max-width: 350px;

  .input {
    width: 350px;
  }

  .label > div {
    display: inline-flex;
    margin-left: 4px;
  }
`;

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
    <StyledWrapper>
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
    </StyledWrapper>
  );
};

export default observer(MetadataUrlField);
