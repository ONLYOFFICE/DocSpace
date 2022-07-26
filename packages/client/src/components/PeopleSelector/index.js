import React from "react";
import { inject, observer } from "mobx-react";
import PropTypes from "prop-types";
import { I18nextProvider, withTranslation } from "react-i18next";
import i18n from "./i18n";
import AdvancedSelector from "@docspace/common/components/AdvancedSelector";
import { getUserList } from "@docspace/common/api/people";
import { getGroupList } from "@docspace/common/api/groups";
import Filter from "@docspace/common/api/people/filter";
import UserTooltip from "./sub-components/UserTooltip";

class PeopleSelector extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      options: [],
      groups: [],
      total: 0,
      page: 0,
      hasNextPage: true,
      isNextPageLoading: false,
      isFirstLoad: true,
    };
  }

  componentDidMount() {
    const { groupList, useFake, t } = this.props;

    if (!groupList) {
      getGroupList(useFake)
        .then((groups) =>
          this.setState({
            groups: [
              {
                key: "all",
                id: "all",
                label: `${t("AllUsers")}`,
                total: 0,
                selectedCount: 0,
              },
            ].concat(this.convertGroups(groups)),
          })
        )
        .catch((error) => console.log(error));
    } else {
      this.setState({
        groups: [
          {
            key: "all",
            id: "all",
            label: `${t("AllUsers")}`,
            total: 0,
            selectedCount: 0,
          },
        ].concat(groupList),
      });
    }
  }

  convertGroups = (groups) => {
    return groups
      ? groups.map((g) => {
          return {
            key: g.id,
            id: g.id,
            label: g.name,
            total: 0,
            selectedCount: 0,
          };
        })
      : [];
  };

  convertUser = (u) => {
    return {
      key: u.id,
      groups: u.groups && u.groups.length ? u.groups.map((g) => g.id) : [],
      label: u.displayName,
      email: u.email,
      position: u.title,
      avatarUrl: u.avatar,
    };
  };

  convertUsers = (users) => {
    return users ? users.map(this.convertUser) : [];
  };

  loadNextPage = ({ startIndex, searchValue, currentGroup }) => {
    // console.log(
    //   `loadNextPage(startIndex=${startIndex}, searchValue="${searchValue}", currentGroup="${currentGroup}")`
    // );

    const pageCount = 100;

    this.setState(
      { isNextPageLoading: true, isFirstLoad: startIndex === 0 },
      () => {
        const { role, employeeStatus, useFake } = this.props;

        const filter = Filter.getDefault();
        filter.page = startIndex / pageCount;
        filter.pageCount = pageCount;

        if (searchValue) {
          filter.search = searchValue;
        }

        if (employeeStatus) {
          filter.employeeStatus = employeeStatus;
        }

        if (role) {
          filter.role = role;
        }
        if (employeeStatus) {
          filter.employeeStatus = employeeStatus;
        }

        if (currentGroup && currentGroup !== "all") filter.group = currentGroup;

        const { defaultOption, defaultOptionLabel } = this.props;

        getUserList(filter, useFake)
          .then((response) => {
            let newOptions = startIndex ? [...this.state.options] : [];

            if (defaultOption) {
              const inGroup =
                !currentGroup ||
                currentGroup === "all" ||
                (defaultOption.groups &&
                  defaultOption.groups.filter((g) => g.id === currentGroup)
                    .length > 0);

              if (searchValue) {
                const exists = response.items.find(
                  (item) => item.id === defaultOption.id
                );

                if (exists && inGroup) {
                  newOptions.push(
                    this.convertUser({
                      ...defaultOption,
                      displayName: defaultOptionLabel,
                    })
                  );
                }
              } else if (!startIndex && response.items.length > 0 && inGroup) {
                newOptions.push(
                  this.convertUser({
                    ...defaultOption,
                    displayName: defaultOptionLabel,
                  })
                );
              }

              newOptions = newOptions.concat(
                this.convertUsers(
                  response.items.filter((item) => item.id !== defaultOption.id)
                )
              );
            } else {
              newOptions = newOptions.concat(this.convertUsers(response.items));
            }

            this.setState({
              hasNextPage: newOptions.length < response.total,
              isNextPageLoading: false,
              options: newOptions,
              total: response.total,
            });
          })
          .catch((error) => console.log(error));
      }
    );
  };

  getOptionTooltipContent = (index) => {
    if (!index) return null;

    const { options } = this.state;

    const user = options[+index];

    if (!user) return null;

    // console.log("onOptionTooltipShow", index, user);

    const { defaultOption, theme } = this.props;

    const label =
      defaultOption && defaultOption.id === user.key
        ? defaultOption.displayName
        : user.label;

    return (
      <UserTooltip
        theme={theme}
        avatarUrl={user.avatarUrl}
        label={label}
        email={user.email}
        position={user.position}
      />
    );
  };

  onSearchChanged = () => {
    //console.log("onSearchChanged")(value);
    this.setState({ options: [], hasNextPage: true, isFirstLoad: true });
  };

  onGroupChanged = () => {
    //console.log("onGroupChanged")(group);
    this.setState({ options: [], hasNextPage: true, isFirstLoad: true });
  };

  render() {
    const {
      options,
      groups,
      hasNextPage,
      isNextPageLoading,
      total,
      isFirstLoad,
    } = this.state;

    const {
      id,
      className,
      style,
      isOpen,
      isMultiSelect,
      isDisabled,
      onSelect,
      size,
      onCancel,
      t,
      searchPlaceHolderLabel,
      withoutAside,
      embeddedComponent,
      selectedOptions,
      showCounter,
      smallSectionWidth,
      theme,
      onArrowClick,
      headerLabel,
    } = this.props;

    // console.log("CustomAllGroups", t("CustomAllGroups", { groupsCaption }));

    // console.log("PeopleSelector render");
    return (
      <AdvancedSelector
        theme={theme}
        id={id}
        className={className}
        style={style}
        options={options}
        groups={groups}
        hasNextPage={hasNextPage}
        isNextPageLoading={isNextPageLoading}
        smallSectionWidth={smallSectionWidth}
        loadNextPage={this.loadNextPage}
        size={size}
        selectedOptions={selectedOptions}
        isOpen={isOpen}
        isMultiSelect={isMultiSelect}
        isDisabled={isDisabled}
        searchPlaceHolderLabel={
          searchPlaceHolderLabel || t("SearchUsersPlaceholder")
        }
        isDefaultDisplayDropDown={false}
        selectButtonLabel={t("PeopleTranslations:AddMembers")}
        emptySearchOptionsLabel={t("EmptySearchUsersResult")}
        emptyOptionsLabel={t("EmptyUsers")}
        onSelect={onSelect}
        onSearchChanged={this.onSearchChanged}
        onGroupChanged={this.onGroupChanged}
        onCancel={onCancel}
        withoutAside={withoutAside}
        embeddedComponent={embeddedComponent}
        showCounter={showCounter}
        onArrowClick={onArrowClick}
        headerLabel={headerLabel ? headerLabel : `${t("Common:AddUsers")}`}
        total={total}
        isFirstLoad={isFirstLoad}
      />
    );
  }
}

PeopleSelector.propTypes = {
  id: PropTypes.string,
  className: PropTypes.oneOfType([PropTypes.string, PropTypes.array]),
  style: PropTypes.object,
  isOpen: PropTypes.bool,
  onSelect: PropTypes.func,
  onCancel: PropTypes.func,
  useFake: PropTypes.bool,
  isMultiSelect: PropTypes.bool,
  isDisabled: PropTypes.bool,
  defaultOption: PropTypes.object,
  defaultOptionLabel: PropTypes.string,
  size: PropTypes.oneOf(["full", "compact"]),
  language: PropTypes.string,
  t: PropTypes.func,
  groupsCaption: PropTypes.string,
  searchPlaceHolderLabel: PropTypes.string,
  role: PropTypes.oneOf(["admin", "user", "guest"]),
  employeeStatus: PropTypes.any,
  withoutAside: PropTypes.bool,
  embeddedComponent: PropTypes.any,
};

PeopleSelector.defaultProps = {
  useFake: false,
  size: "full",
  language: "en",
  role: null,
  employeeStatus: null,
  defaultOption: null,
  defaultOptionLabel: "Me",
  withoutAside: false,
};

const ExtendedPeopleSelector = inject(({ auth }) => {
  return { theme: auth.settingsStore.theme };
})(
  observer(
    withTranslation(["PeopleSelector", "PeopleTranslations", "Common"])(
      PeopleSelector
    )
  )
);

export default (props) => (
  <I18nextProvider i18n={i18n}>
    <ExtendedPeopleSelector {...props} />
  </I18nextProvider>
);
