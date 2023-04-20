import React, { useEffect, useState } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
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
      history.push("/");
    } catch (error) {
      toastr.error(e);
      console.error(error);
    }
  };

  const onCancelClick = () => {
    history.push("/");
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
  withRouter(
    withTranslation(["Confirm", "Common"])(
      withLoader(observer(ChangeOwnerForm))
    )
  )
);
