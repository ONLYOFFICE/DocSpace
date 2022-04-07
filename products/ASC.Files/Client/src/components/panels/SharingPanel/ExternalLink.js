import React from "react";

import ToggleButton from "@appserver/components/toggle-button";
import LinkWithDropdown from "@appserver/components/link-with-dropdown";
import InputBlock from "@appserver/components/input-block";
import IconButton from "@appserver/components/icon-button";
import AccessComboBox from "./AccessComboBox";

import { ShareAccessRights } from "@appserver/common/constants";

import CopyIcon from "../../../../../../../public/images/copy.react.svg";
import CodeIcon from "../../../../../../../public/images/code.react.svg";

import { StyledExternalLink } from "./StyledSharingPanel";

const ExternalLink = ({
  t,
  linkText,
  shareText,
  shareLink,
  options,
  externalAccessOptions,
  onToggleLink,
  onCopyLink,
  onShowEmbeddingPanel,
  onChangeItemAccess,
  item,
}) => {
  const [isChecked, setIsChecked] = React.useState(false);

  React.useEffect(() => {
    setIsChecked(item.access !== ShareAccessRights.DenyAccess);
  }, [item]);

  const onToggleLinkAction = React.useCallback(() => {
    setIsChecked((val) => !val);
    onToggleLink && onToggleLink(item);
  }, [item, onToggleLink]);

  const onCopyLinkAction = React.useCallback(() => {
    onCopyLink && onCopyLink();
  }, [onCopyLink]);

  const onShowEmbeddingPanelAction = React.useCallback(() => {
    onShowEmbeddingPanel && onShowEmbeddingPanel();
  }, [onShowEmbeddingPanel]);

  return (
    <StyledExternalLink>
      <div className="external-link__unchecked">
        <ToggleButton
          className="external-link__toggler"
          label={linkText}
          isChecked={isChecked}
          onChange={onToggleLinkAction}
        />

        {isChecked && (
          <LinkWithDropdown
            className="external-link__share"
            dropdownType="alwaysDashed"
            data={options}
            fontSize="13px"
            fontWeight={600}
            directionX={"right"}
            directionY={"bottom"}
            isDefaultMode={false}
          >
            {shareText}
          </LinkWithDropdown>
        )}
      </div>

      {isChecked && (
        <div className="external-link__checked">
          <InputBlock
            className="external-link__input-link"
            scale={true}
            isReadOnly={true}
            placeholder={shareLink}
          >
            <div className="external-link__buttons">
              <CodeIcon
                className="external-link__code"
                onClick={onShowEmbeddingPanelAction}
              />
              <CopyIcon
                className="external-link__copy"
                onClick={onCopyLinkAction}
              />
            </div>
          </InputBlock>

          <AccessComboBox
            t={t}
            access={item.access}
            directionX="right"
            directionY="bottom"
            accessOptions={externalAccessOptions}
            onAccessChange={onChangeItemAccess}
            itemId={item.sharedTo.id}
            isDisabled={false}
            disableLink={false}
          />
        </div>
      )}
    </StyledExternalLink>
  );
};

export default ExternalLink;
