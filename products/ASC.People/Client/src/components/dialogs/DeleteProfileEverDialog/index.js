import React from "react";
import { withRouter } from "react-router";
import PropTypes from "prop-types";

import Button from "@appserver/components/button";
import ModalDialog from "@appserver/components/modal-dialog";
import Text from "@appserver/components/text";

import { withTranslation, Trans } from "react-i18next";
import api from "@appserver/common/api";
import toastr from "studio/toastr";
import ModalDialogContainer from "../ModalDialogContainer";
import { inject, observer } from "mobx-react";
import config from "../../../../package.json";
import { combineUrl } from "@appserver/common/utils";
import { AppServerConfig } from "@appserver/common/constants";

const { deleteUser } = api.people; //TODO: Move to action
const { Filter } = api;

class DeleteProfileEverDialogComponent extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      isRequestRunning: false,
    };
  }
  onDeleteProfileEver = () => {
    const { user, t, history, homepage, setFilter } = this.props;

    const filter = Filter.getDefault();
    const params = filter.toUrlParams();

    const url = combineUrl(
      AppServerConfig.proxyURL,
      homepage,
      `filter?/${params}`
    );

    this.setState({ isRequestRunning: true }, () => {
      deleteUser(user.id)
        .then((res) => {
          toastr.success(t("SuccessfullyDeleteUserInfoMessage"));
          history.push(url, params);
          setFilter(filter);
          return;
        })
        .catch((error) => toastr.error(error));
    });
  };

  onReassignDataClick = () => {
    const { homepage, user } = this.props;
    history.push(
      combineUrl(
        AppServerConfig.proxyURL,
        homepage,
        `/reassign/${user.userName}`
      )
    );
  };

  render() {
    console.log("DeleteProfileEverDialog render");
    const { t, tReady, visible, user, onClose, userCaption } = this.props;
    const { isRequestRunning } = this.state;

    return (
      <ModalDialogContainer
        isLoading={!tReady}
        visible={visible}
        onClose={onClose}
      >
        <ModalDialog.Header>{t("Common:Confirmation")}</ModalDialog.Header>
        <ModalDialog.Body>
          <Text>
            <Trans
              i18nKey="DeleteUserConfirmation"
              ns="DeleteProfileEverDialog"
              t={t}
            >
              {{ userCaption }} <strong>{{ user: user.displayName }}</strong>{" "}
              will be deleted.
            </Trans>
          </Text>
          <Text>{t("Translations:NotBeUndone")}</Text>
          {/* <Text color="#c30" fontSize="18px" className="warning-text">
              {t("Common:Warning")}!
            </Text>
            <Text>{t("DeleteUserDataConfirmation")}</Text> */}
        </ModalDialog.Body>
        <ModalDialog.Footer>
          <Button
            key="OKBtn"
            className="delete-profile_button-delete"
            label={t("Common:OKButton")}
            size="small"
            primary={true}
            onClick={this.onDeleteProfileEver}
            isLoading={isRequestRunning}
          />
          {/* <Button
              className="button-dialog"
              key="ReassignBtn"
              label={t("Translations:ReassignData")}
              size="small"
              onClick={this.onReassignDataClick}
              isDisabled={isRequestRunning}
            /> */}
        </ModalDialog.Footer>
      </ModalDialogContainer>
    );
  }
}

const DeleteProfileEverDialog = withTranslation([
  "DeleteProfileEverDialog",
  "Common",
  "Translations",
])(DeleteProfileEverDialogComponent);

DeleteProfileEverDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  user: PropTypes.object.isRequired,
};

export default withRouter(
  inject(({ auth, peopleStore }) => ({
    homepage: config.homepage,
    userCaption: auth.settingsStore.customNames.userCaption,
    setFilter: peopleStore.filterStore.setFilterParams,
  }))(observer(DeleteProfileEverDialog))
);
