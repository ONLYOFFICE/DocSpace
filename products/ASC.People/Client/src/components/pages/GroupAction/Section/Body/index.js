import React from "react";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router";
import styled from 'styled-components';
import PropTypes from "prop-types";
import {
  Button,
  TextInput,
  Text,
  Icons,
  SelectedItem,
  AdvancedSelector,
  FieldContainer,
  ComboBox,
  ModalDialog,
  SearchInput
} from "asc-web-components";
import {
  department,
  headOfDepartment,
  typeUser
} from "../../../../../helpers/customNames";
import { connect } from "react-redux";
import { resetGroup } from "../../../../../store/group/actions";

const StyledBodyContainer = styled.div`
  .group-name_input {
    width: 320px;
  }

  .head_container,
  .members_container { 
    margin-top: 16px; 
    position: "relative";
  }

  .search_container {
    margin-top: 16px;
  }

  .selected-members_container { 
    margin-top: 16px;
    display: "flex";
    flex-wrap: "wrap";
    flex-direction: "row";

    .selected-item { 
      margin-right: 8px; 
      margin-bottom: 8px;
    }
  }

  .buttons_container { 
    margin-top: 60px;

    .cancel-button { 
      margin-left: 8px;
    }
  }
`;

class SectionBodyContent extends React.Component {
  constructor(props) {
    super(props);

    const { group, users, groups, t } = props;

    this.state = {
      id: group ? group.id : "",
      groupName: group ? group.name : "",
      searchValue: "",
      error: null,
      inLoading: false,
      isHeaderSelectorOpen: false,
      isUsersSelectorOpen: false,
      users: users,
      groups: groups,
      modalVisible: false,
      header: group
        ? {
            key: 0,
            label: "{SELECTED HEADER NAME}" //group.head
          }
        : {
            key: 0,
            label: t("CustomAddEmployee", { typeUser })
          },
      groupMembers: group && group.members ? group.members : []
    };
  }

  onGroupChange = e => {
    this.setState({
      groupName: e.target.value
    });
  };

  onSearchChange = e => {
    this.setState({
      searchValue: e.target.value
    });
  };

  onHeaderSelectorClick = () => {
    this.setState({
      isHeaderSelectorOpen: !this.state.isHeaderSelectorOpen
    });
  };

  onUsersSelectorClick = () => {
    this.setState({
      isUsersSelectorOpen: !this.state.isUsersSelectorOpen
    });
  };

  toggleModalVisible = () => {
    this.setState({
      modalVisible: !this.state.modalVisible
    });
  };

  onCancel = () => {
    const { history, resetGroup } = this.props;

    resetGroup();
    history.goBack();
  };

  render() {
    const { t } = this.props;
    const {
      groupName,
      users,
      groups,
      groupMembers,
      isHeaderSelectorOpen,
      isUsersSelectorOpen,
      modalVisible,
      inLoading,
      error,
      searchValue
    } = this.state;

    return (
      <StyledBodyContainer>
        <div>
          <label htmlFor="group-name">
            <Text.Body as="span" isBold={true}>
              {t("CustomDepartmentName", { department })}:
            </Text.Body>
          </label>
          <div className="group-name_input">
            <TextInput
              id="group-name"
              name="group-name"
              scale={true}
              isAutoFocussed={true}
              tabIndex={1}
              value={groupName}
              onChange={this.onGroupChange}
            />
          </div>
        </div>
        <div className="head_container">
          <label htmlFor="head-selector">
            <Text.Body as="span" isBold={true}>
              {t("CustomHeadOfDepartment", { headOfDepartment })}:
            </Text.Body>
          </label>
          <ComboBox
            id="head-selector"
            tabIndex={2}
            options={[]}
            advancedOptions={
              <AdvancedSelector
                isDropDown={true}
                isOpen={true}
                size="full"
                placeholder={"Search"}
                onSearchChanged={value => {
                  /*setOptions(
                options.filter(option => {
                  return option.label.indexOf(value) > -1;
                })
              );*/
                }}
                options={users}
                groups={groups}
                isMultiSelect={false}
                buttonLabel={t("CustomAddEmployee", { typeUser })}
                selectAllLabel={"Select all"}
                onSelect={selectedOptions => {
                  // action('onSelect')(selectedOptions);
                  //toggle();
                }}
                //onCancel={toggle}
                allowCreation={false}
                //onAddNewClick={toggleModalVisible}
                allowAnyClickClose={true}
              />
            }
            //onSelect={option => action('Selected option')(option)}
            selectedOption={{
              key: 0,
              label: t("CustomAddEmployee", { typeUser })
            }}
            //</div>isDisabled={boolean('isDisabled', false)}
            scaled={false}
            size="content"
            opened={isHeaderSelectorOpen}
            onClick={this.onHeaderSelectorClick}
          >
            <Icons.CatalogGuestIcon size="medium" />
          </ComboBox>
        </div>
        <div className="members_container">
          <label htmlFor="employee-selector">
            <Text.Body as="span" isBold={true}>
              Members:
            </Text.Body>
          </label>
          <ComboBox
            id="employee-selector"
            tabIndex={3}
            options={[]}
            advancedOptions={
              <AdvancedSelector
                isDropDown={true}
                isOpen={true}
                size="full"
                placeholder={"Search"}
                onSearchChanged={value => {
                  /*setOptions(
                options.filter(option => {
                  return option.label.indexOf(value) > -1;
                })
              );*/
                }}
                options={users}
                groups={groups}
                isMultiSelect={true}
                buttonLabel={t("CustomAddEmployee", { typeUser })}
                selectAllLabel={"Select all"}
                onSelect={selectedOptions => {
                  /*console.log('onSelect', selectedOptions);
              toggle();*/
                }}
                //onCancel={toggle}
                allowCreation={false}
                //onAddNewClick={toggleModalVisible}
                allowAnyClickClose={true}
              />
            }
            //onSelect={option => action('Selected option')(option)}
            selectedOption={{
              key: 0,
              label: t("CustomAddEmployee", { typeUser })
            }}
            //</div>isDisabled={boolean('isDisabled', false)}
            scaled={false}
            size="content"
            opened={isUsersSelectorOpen}
            onClick={this.onUsersSelectorClick}
          >
            <Icons.CatalogGuestIcon size="medium" />
          </ComboBox>
          <ModalDialog
            zIndex={1001}
            visible={modalVisible}
            headerContent="New User"
            bodyContent={
              <div className="create_new_user_modal">
                <FieldContainer
                  isVertical={true}
                  isRequired={true}
                  hasError={false}
                  labelText={"First name:"}
                >
                  <TextInput
                    value={""}
                    hasError={false}
                    className="firstName-input"
                    scale={true}
                    autoComplete="off"
                    onChange={e => {
                      //set(e.target.value);
                    }}
                  />
                </FieldContainer>
                <FieldContainer
                  isVertical={true}
                  isRequired={true}
                  hasError={false}
                  labelText={"Last name:"}
                >
                  <TextInput
                    value={""}
                    hasError={false}
                    className="lastName-input"
                    scale={true}
                    autoComplete="off"
                    onChange={e => {
                      //set(e.target.value);
                    }}
                  />
                </FieldContainer>
                <FieldContainer
                  isVertical={true}
                  isRequired={true}
                  hasError={false}
                  labelText={"E-mail:"}
                >
                  <TextInput
                    value={""}
                    hasError={false}
                    className="email-input"
                    scale={true}
                    autoComplete="off"
                    onChange={e => {
                      //set(e.target.value);
                    }}
                  />
                </FieldContainer>
                <FieldContainer
                  isVertical={true}
                  isRequired={true}
                  hasError={false}
                  labelText={"Group:"}
                >
                  <ComboBox
                    options={groups}
                    className="group-input"
                    onSelect={option => console.log("Selected option", option)}
                    selectedOption={{
                      key: 0,
                      label: "Select"
                    }}
                    dropDownMaxHeight={200}
                    scaled={true}
                    scaledOptions={true}
                    size="content"
                  />
                </FieldContainer>
              </div>
            }
            footerContent={[
              <Button
                key="CreateBtn"
                label="Create"
                primary={true}
                size="big"
                onClick={e => {
                  console.log("CreateBtn click", e);
                  this.toggleModalVisible();
                }}
              />
            ]}
            onClose={this.toggleModalVisible}
          />
        </div>
        <div className="search_container">
          <SearchInput
            id="member-search"
            isDisabled={inLoading}
            size='base'
            scale={true}
            placeholder='Search'
            value={searchValue}
            onChange={this.onSearchChange}
          />
        </div>
        <div className="selected-members_container">
          {groupMembers.map(member => (
            <SelectedItem
              key={member.id}
              text={member.displayName}
              onClick={e => console.log("onClick", e.target)}
              onClose={e => console.log("onClose", e.target)}
              isInline={true}
              className="selected-item"
            />
          ))}
        </div>
        <div>{error && <strong>{error}</strong>}</div>
        <div className="buttons_container">
          <Button
            label={t("SaveButton")}
            primary
            type="submit"
            isLoading={inLoading}
            size="big"
            tabIndex={4}
          />
          <Button
            label={t("CancelButton")}
            className="cancel-button"
            size="big"
            isDisabled={inLoading}
            onClick={this.onCancel}
            tabIndex={5}
          />
        </div>
      </StyledBodyContainer>
    );
  }
}

SectionBodyContent.propTypes = {
  group: PropTypes.object
};

SectionBodyContent.defaultProps = {
  group: null
};

const convertUsers = users => {
  return users
    ? users.map(u => {
        return {
          key: u.id,
          groups: [],
          label: u.displayName
        };
      })
    : [];
};

const convertGroups = groups => {
  return groups
    ? groups.map(g => {
        return {
          key: g.id,
          label: g.name,
          total: 0
        };
      })
    : [];
};

function mapStateToProps(state) {
  return {
    settings: state.auth.settings,
    group: state.group.targetGroup,
    groups: convertGroups(state.people.groups),
    users: convertUsers(state.people.users) //TODO: replace to api requests with search
  };
}

export default connect(
  mapStateToProps,
  { resetGroup }
)(withRouter(withTranslation()(SectionBodyContent)));
