import React, { useCallback, useState } from "react";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import PropTypes from "prop-types";
import {
  toastr,
  ModalDialog,
  Button,
  Text,
  ToggleContent
} from "asc-web-components";
import { withTranslation } from "react-i18next";
import i18n from "./i18n";
import { utils } from "asc-web-common";
import ModalDialogContainer from "../ModalDialogContainer";
import { updateUserType } from "../../../store/people/actions";

const { changeLanguage } = utils;

const ChangeUserTypeDialogComponent = props => {
  const { t, onClose, visible, users, userType } = props;
  const usersId = [];
  users.map(item => usersId.push(item.id));

  const [isRequestRunning, setIsRequestRunning] = useState(false);

  changeLanguage(i18n);

  const onChangeUserType = useCallback(() => {
    setIsRequestRunning(true);
    updateUserType(userType, usersId)
      .then(() => toastr.success(t("SuccessChangeUserType")))
      .catch(error => toastr.error(error))
      .finally(() => {
        setIsRequestRunning(false);
        onClose();
      });
  }, [t, onClose, usersId, userType]);

  const firstType = userType === 1 ? t("GuestCaption") : t("UserCol");
  const secondType = userType === 1 ? t("UserCol") : t("GuestCaption");

  return (
    <ModalDialogContainer>
      <ModalDialog
        visible={visible}
        onClose={onClose}
        headerContent={t("ChangeUserTypeHeader")}
        bodyContent={
          <>
            <Text>
              {t("ChangeUserTypeMessage", {
                firstType: firstType,
                secondType: secondType
              })}
            </Text>

            <ToggleContent
              className="toggle-content-dialog"
              label={t("ShowUsersList")}
            >
              {users.map((item, index) => (
                <Text key={index}>{item.displayName}</Text>
              ))}
            </ToggleContent>
          </>
        }
        footerContent={
          <>
            <Button
              label={t("ChangeUserTypeButton")}
              size="medium"
              primary
              onClick={onChangeUserType}
              isLoading={isRequestRunning}
            />
            <Button
              className="button-dialog"
              label={t("CancelButton")}
              size="medium"
              onClick={onClose}
              isDisabled={isRequestRunning}
            />
          </>
        }
      />
    </ModalDialogContainer>
  );
};

const ChangeUserTypeDialogTranslated = withTranslation()(
  ChangeUserTypeDialogComponent
);

const ChangeUserTypeDialog = props => (
  <ChangeUserTypeDialogTranslated i18n={i18n} {...props} />
);

ChangeUserTypeDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  users: PropTypes.arrayOf(PropTypes.object).isRequired
};

export default connect(null, { updateUserType })(
  withRouter(ChangeUserTypeDialog)
);
