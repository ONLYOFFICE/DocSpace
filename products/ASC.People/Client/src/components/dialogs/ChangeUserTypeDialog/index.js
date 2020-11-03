import React, { memo } from "react";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import PropTypes from "prop-types";
import {
  ModalDialog,
  Button,
  Text,
  ToggleContent,
  Checkbox,
  CustomScrollbarsVirtualList,
} from "asc-web-components";
import { withTranslation } from "react-i18next";
import { FixedSizeList as List, areEqual } from "react-window";
import AutoSizer from "react-virtualized-auto-sizer";
import { utils, toastr, constants } from "asc-web-common";
import ModalDialogContainer from "../ModalDialogContainer";
import { updateUserType, setSelected } from "../../../store/people/actions";
import { createI18N } from "../../../helpers/i18n";
import {
  getUsersToMakeEmployeesIds,
  getUsersToMakeGuestsIds,
} from "../../../store/people/selectors";

const i18n = createI18N({
  page: "ChangeUserTypeDialog",
  localesPath: "dialogs/ChangeUserTypeDialog",
});

const { EmployeeType } = constants;

const { changeLanguage } = utils;

class ChangeUserTypeDialogComponent extends React.Component {
  constructor(props) {
    super(props);

    changeLanguage(i18n);

    const { selectedUsers, userIds } = props;

    const listUsers = selectedUsers.map((item, index) => {
      const disabled = userIds.find((x) => x === item.id);
      return (selectedUsers[index] = {
        ...selectedUsers[index],
        checked: disabled ? true : false,
        disabled: disabled ? false : true,
      });
    });

    this.state = { isRequestRunning: false, userIds, listUsers };
  }

  onChange = (e) => {
    const { listUsers } = this.state;
    const userIndex = listUsers.findIndex((x) => x.id === e.target.value);
    const newUsersList = listUsers;
    newUsersList[userIndex].checked = !newUsersList[userIndex].checked;

    const newUserIds = [];
    for (let item of newUsersList) {
      if (item.checked === true) {
        newUserIds.push(item.id);
      }
    }

    this.setState({ listUsers: newUsersList, userIds: newUserIds });
  };

  onChangeUserType = () => {
    const { onClose, setSelected, t, userType, updateUserType } = this.props;
    const { userIds } = this.state;
    this.setState({ isRequestRunning: true }, () => {
      updateUserType(userType, userIds)
        .then(() => toastr.success(t("SuccessChangeUserType")))
        .catch((error) => toastr.error(error))
        .finally(() => {
          this.setState({ isRequestRunning: false }, () => {
            setSelected("close");
            onClose();
          });
        });
    });
  };

  render() {
    const { visible, onClose, t, userType } = this.props;
    const { isRequestRunning, listUsers, userIds } = this.state;
    const itemSize = 25;
    const containerStyles = { height: listUsers.length * 25, maxHeight: 220 };

    const renderItems = memo(({ data, index, style }) => {
      return (
        <Checkbox
          truncate
          style={style}
          className="modal-dialog-checkbox"
          value={data[index].id}
          onChange={this.onChange}
          key={`checkbox_${index}`}
          isChecked={data[index].checked}
          label={data[index].displayName}
          isDisabled={data[index].disabled}
        />
      );
    }, areEqual);

    const renderList = ({ height, width }) => (
      <List
        className="List"
        height={height}
        width={width}
        itemSize={itemSize}
        itemCount={listUsers.length}
        itemData={listUsers}
        outerElementType={CustomScrollbarsVirtualList}
      >
        {renderItems}
      </List>
    );

    const firstType = userType === 1 ? t("GuestCaption") : t("UserCol");
    const secondType = userType === 1 ? t("UserCol") : t("GuestCaption");
    return (
      <ModalDialogContainer>
        <ModalDialog visible={visible} onClose={onClose}>
          <ModalDialog.Header>{t("ChangeUserTypeHeader")}</ModalDialog.Header>
          <ModalDialog.Body>
            <Text>
              {t("ChangeUserTypeMessage", {
                firstType: firstType,
                secondType: secondType,
              })}
            </Text>
            <Text>{t("ChangeUserTypeMessageWarning")}</Text>

            <ToggleContent
              className="toggle-content-dialog"
              label={t("ShowUsersList")}
            >
              <div style={containerStyles} className="modal-dialog-content">
                <AutoSizer>{renderList}</AutoSizer>
              </div>
            </ToggleContent>
          </ModalDialog.Body>
          <ModalDialog.Footer>
            <Button
              label={t("ChangeUserTypeButton")}
              size="medium"
              primary
              onClick={this.onChangeUserType}
              isLoading={isRequestRunning}
              isDisabled={!userIds.length}
            />
            <Button
              className="button-dialog"
              label={t("CancelButton")}
              size="medium"
              onClick={onClose}
              isDisabled={isRequestRunning}
            />
          </ModalDialog.Footer>
        </ModalDialog>
      </ModalDialogContainer>
    );
  }
}

const ChangeUserTypeDialogTranslated = withTranslation()(
  ChangeUserTypeDialogComponent
);

const ChangeUserTypeDialog = (props) => (
  <ChangeUserTypeDialogTranslated i18n={i18n} {...props} />
);

ChangeUserTypeDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  setSelected: PropTypes.func.isRequired,
  userIds: PropTypes.arrayOf(PropTypes.string).isRequired,
  selectedUsers: PropTypes.arrayOf(PropTypes.object).isRequired,
};

const mapStateToProps = (state, ownProps) => {
  const { selection } = state.people;
  const { userType } = ownProps;

  return {
    userIds:
      userType === EmployeeType.User
        ? getUsersToMakeEmployeesIds(state)
        : getUsersToMakeGuestsIds(state),
    selectedUsers: selection,
  };
};

export default connect(mapStateToProps, { updateUserType, setSelected })(
  withRouter(ChangeUserTypeDialog)
);
