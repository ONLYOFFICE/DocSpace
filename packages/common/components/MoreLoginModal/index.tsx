import React from "react";
import ModalDialog from "@docspace/components/modal-dialog";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import { providersData } from "@docspace/common/constants";
import styled from "styled-components";
import { ReactSVG } from "react-svg";
import { getProviderTranslation } from "@docspace/common/utils";
import SsoReactSvgUrl from "PUBLIC_DIR/images/sso.react.svg?url";

const ProviderRow = styled.div`
  width: 100%;
  display: flex;
  flex-direction: row;
  flex-wrap: nowrap;

  justify-content: flex-start;
  align-items: center;
  align-content: center;
  padding: 8px 0;

  svg {
    height: 24px;
    width: 24px;
    padding-left: 4px;

    path {
      fill: ${(props) => !props.theme.isBase && "#fff"};
    }
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

interface IMoreLoginNodalProps {
  visible: boolean;
  onClose: VoidFunction;
  providers: ProvidersType;
  onSocialLoginClick: (
    e: HTMLElementEvent<HTMLButtonElement | HTMLElement>
  ) => void;
  ssoLabel: string;
  ssoUrl: string;
  t: TFuncType;
}

const MoreLoginModal: React.FC<IMoreLoginNodalProps> = (props) => {
  const {
    t,
    visible,
    onClose,
    providers,
    onSocialLoginClick,
    ssoLabel,
    ssoUrl,
  } = props;

  console.log("more login render", props);

  return (
    <Modal
      displayType="aside"
      visible={visible}
      onClose={onClose}
      removeScroll={true}
    >
      <ModalDialog.Header>{t("Common:Authorization")}</ModalDialog.Header>
      <ModalDialog.Body>
        {ssoUrl && (
          <ProviderRow key={`ProviderItemSSO`}>
            <ReactSVG src={SsoReactSvgUrl} />
            <Text
              fontSize="14px"
              fontWeight="600"
              className="provider-name"
              noSelect
            >
              {ssoLabel || getProviderTranslation("sso", t)}
            </Text>
            <Button
              label={t("Common:LoginButton")}
              className="signin-button"
              size="small"
              onClick={() => (window.location.href = ssoUrl)}
            />
          </ProviderRow>
        )}
        {providers?.map((item, index) => {
          if (!providersData[item.provider]) return;

          const { icon, label } = providersData[item.provider];

          return (
            <ProviderRow key={`ProviderItem${index}`}>
              <ReactSVG src={icon} />
              <Text
                fontSize="14px"
                fontWeight="600"
                className="provider-name"
                noSelect
              >
                {getProviderTranslation(label, t)}
              </Text>
              <Button
                label={t("Common:LoginButton")}
                className="signin-button"
                size="small"
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
