import React, { useEffect, useState } from "react";
import { withTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import { inject, observer } from "mobx-react";
import {
  StyledPage,
  StyledBody,
  ButtonsWrapper,
  StyledContent,
} from "./StyledConfirm";
import withLoader from "../withLoader";
import FormWrapper from "@docspace/components/form-wrapper";
import toastr from "@docspace/components/toast/toastr";
import DocspaceLogo from "../../../DocspaceLogo";
import { ownerChange } from "@docspace/common/api/settings";
import { getUserFromConfirm } from "@docspace/common/api/people";

const ChangeOwnerForm = (props) => {
  const { t, greetingTitle, linkData, history } = props;
  const [newOwner, setNewOwner] = useState("");
  const [isOwnerChanged, setIsOwnerChanged] = useState(false);

  const navigate = useNavigate();
  const ownerId = linkData.uid;

  useEffect(() => {
    const fetchData = async () => {
      const confirmKey = linkData.confirmHeader;
      const user = await getUserFromConfirm(ownerId, confirmKey);
      setNewOwner(user?.displayName);
    };

    fetchData();
  }, []);

  const onChangeOwnerClick = async () => {
    try {
      await ownerChange(ownerId, linkData.confirmHeader);
      setIsOwnerChanged(true);
      setTimeout(() => (location.href = "/"), 10000);
    } catch (error) {
      toastr.error(e);
      console.error(error);
    }
  };

  const onCancelClick = () => {
    navigate("/");
  };

  return (
    <StyledPage>
      <StyledContent>
        <StyledBody>
          <DocspaceLogo className="docspace-logo" />
          <Text fontSize="23px" fontWeight="700" className="title">
            {greetingTitle}
          </Text>

          <FormWrapper>
            {isOwnerChanged ? (
              <Text>{t("ConfirmOwnerPortalSuccessMessage")}</Text>
            ) : (
              <>
                <Text className="subtitle">
                  {t("ConfirmOwnerPortalTitle", { newOwner: newOwner })}
                </Text>
                <ButtonsWrapper>
                  <Button
                    primary
                    scale
                    size="medium"
                    label={t("Common:SaveButton")}
                    tabIndex={2}
                    isDisabled={false}
                    onClick={onChangeOwnerClick}
                  />
                  <Button
                    scale
                    size="medium"
                    label={t("Common:CancelButton")}
                    tabIndex={2}
                    isDisabled={false}
                    onClick={onCancelClick}
                  />
                </ButtonsWrapper>
              </>
            )}
          </FormWrapper>
        </StyledBody>
      </StyledContent>
    </StyledPage>
  );
};

export default inject(({ auth }) => ({
  greetingTitle: auth.settingsStore.greetingSettings,
  defaultPage: auth.settingsStore.defaultPage,
}))(
  withTranslation(["Confirm", "Common"])(withLoader(observer(ChangeOwnerForm)))
);
