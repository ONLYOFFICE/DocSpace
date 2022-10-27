import React, { memo } from "react";
import { withRouter } from "react-router";
import PropTypes from "prop-types";

import Button from "@docspace/components/button";
import ModalDialog from "@docspace/components/modal-dialog";
import Text from "@docspace/components/text";

import { withTranslation, Trans } from "react-i18next";

import toastr from "@docspace/components/toast/toastr";

import ModalDialogContainer from "../ModalDialogContainer";

import { inject, observer } from "mobx-react";

class ChangeUserTypeDialogComponent extends React.Component {
  constructor(props) {
    super(props);

    const { userIDs } = props;

    this.state = { isRequestRunning: false, userIDs };
  }

  onChangeUserType = () => {
    const {
      onClose,

      t,
      toType,
      fromType,
      updateUserType,
      filter,
    } = this.props;
    const { userIDs } = this.state;
    this.setState({ isRequestRunning: true }, () => {
      updateUserType(toType, userIDs, filter, fromType)
        .then(() => toastr.success(t("SuccessChangeUserType")))
        .catch((error) => toastr.error(error))
        .finally(() => {
          this.setState({ isRequestRunning: false }, () => {
            onClose();
          });
        });
    });
  };

  onCloseAction = async () => {
    const { isRequestRunning } = this.state;
    const { onClose, getUsersList, filter } = this.props;
    if (!isRequestRunning) {
      await getUsersList(filter);

      onClose();
    }
  };

  getType = (type) => {
    const { t } = this.props;

    switch (type) {
      case "admin":
        return t("Common:DocSpaceAdmin");

      case "manager":
        return t("Common:RoomAdmin");

      case "user":
      default:
        return t("Common:User");
    }
  };

  render() {
    const { visible, t, tReady, toType, fromType } = this.props;
    const { isRequestRunning, userIDs } = this.state;

    const firstType = fromType.length === 1 ? this.getType(fromType[0]) : null;
    const secondType = this.getType(toType);

    const changeUserTypeMessage = firstType ? (
      <Trans i18nKey="ChangeUserTypeMessage" ns="ChangeUserTypeDialog" t={t}>
        Users with the <b>'{{ firstType }}'</b> type will be moved to{" "}
        <b>'{{ secondType }}'</b> type.
      </Trans>
    ) : (
      <Trans
        i18nKey="ChangeUserTypeMessageMulti"
        ns="ChangeUserTypeDialog"
        t={t}
      >
        The selected users will be moved to <b>'{{ secondType }}'</b> type.
      </Trans>
    );

    return (
      <ModalDialogContainer
        isLoading={!tReady}
        visible={visible}
        onClose={this.onCloseAction}
        autoMaxHeight
      >
        <ModalDialog.Header>{t("ChangeUserTypeHeader")}</ModalDialog.Header>
        <ModalDialog.Body>
          <Text fontWeight={600}>
            {changeUserTypeMessage} {t("ChangeUserTypeMessageWarning")}
          </Text>
        </ModalDialog.Body>
        <ModalDialog.Footer>
          <Button
            label={t("ChangeUserTypeButton")}
            size="normal"
            scale
            primary
            onClick={this.onChangeUserType}
            isLoading={isRequestRunning}
            isDisabled={!userIDs.length}
          />
          <Button
            label={t("Common:CancelButton")}
            size="normal"
            scale
            onClick={this.onCloseAction}
            isDisabled={isRequestRunning}
          />
        </ModalDialog.Footer>
      </ModalDialogContainer>
    );
  }
}

const ChangeUserTypeDialog = withTranslation([
  "ChangeUserTypeDialog",
  "People",
  "Common",
])(ChangeUserTypeDialogComponent);

ChangeUserTypeDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  userIDs: PropTypes.arrayOf(PropTypes.string).isRequired,
};

export default withRouter(
  inject(({ peopleStore }) => {
    return {
      filter: peopleStore.filterStore.filter,
      updateUserType: peopleStore.usersStore.updateUserType,
      getUsersList: peopleStore.usersStore.getUsersList,
    };
  })(observer(ChangeUserTypeDialog))
);
