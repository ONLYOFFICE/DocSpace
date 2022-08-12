import React from "react";
import ModalDialog from "@docspace/components/modal-dialog";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import { providersData } from "@docspace/common/constants";
import styled from "styled-components";
import { ReactSVG } from "react-svg";
import { getProviderTranslation } from "@docspace/common/utils";

const ProviderRow = styled.div`
  width: 100%;
  display: flex;
  flex-direction: row;
  flex-wrap: nowrap;

  justify-content: flex-start;
  align-items: center;
  align-content: center;
  padding-top: 12px;

  svg {
    height: 24px;
    width: 24px;
    padding-top: 4px;
  }

  .provider-name {
    padding-left: 12px;
    line-height: 16px;
  }

  .signin-button {
    margin-left: auto;
  }
`;

const Modal = styled(ModalDialog)`
  .modal-dialog-aside {
    transform: translateX(${(props) => (props.visible ? "0" : "480px")});
    width: 480px;

    @media (max-width: 375px) {
      width: 325px;
      transform: translateX(${(props) => (props.visible ? "0" : "480px")});
    }
  }
`;

const MoreLoginModal = (props) => {
  const {
    t,
    visible,
    onClose,
    providers,
    onSocialLoginClick,
    ssoLabel,
    ssoUrl,
  } = props;

  return (
    <Modal
      displayType="aside"
      visible={visible}
      onClose={onClose}
      removeScroll={true}
    >
      <ModalDialog.Header>{t("Authorization")}</ModalDialog.Header>
      <ModalDialog.Body>
        {ssoUrl && (
          <ProviderRow key={`ProviderItemSSO`}>
            <ReactSVG src="/static/images/sso.react.svg" />
            <Text fontSize="14px" fontWeight="600" className="provider-name">
              {ssoLabel || getProviderTranslation("sso", t)}
            </Text>
            <Button
              label={t("Common:LoginButton")}
              className="signin-button"
              size="normal"
              onClick={() => (window.location.href = ssoUrl)}
            />
          </ProviderRow>
        )}
        {providers.map((item, index) => {
          if (!providersData[item.provider]) return;

          const { icon, label } = providersData[item.provider];

          return (
            <ProviderRow key={`ProviderItem${index}`}>
              <ReactSVG src={icon} />
              <Text fontSize="14px" fontWeight="600" className="provider-name">
                {getProviderTranslation(label, t)}
              </Text>
              <Button
                label={t("Common:LoginButton")}
                className="signin-button"
                size="normal"
                data-url={item.url}
                data-providername={item.provider}
                onClick={onSocialLoginClick}
              />
            </ProviderRow>
          );
        })}
      </ModalDialog.Body>
    </Modal>
  );
};

export default MoreLoginModal;
