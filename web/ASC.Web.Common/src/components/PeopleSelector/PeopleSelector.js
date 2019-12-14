import React from "react";
import PropTypes from "prop-types";
import { Avatar, Text } from "asc-web-components";
import AdvancedSelector from "../AdvancedSelector";
import { getUserList } from "../../api/people";
import { getGroupList } from "../../api/groups";
import Filter from "../../api/people/filter";

class PeopleSelector extends React.Component {
  constructor(props) {
    super(props);

    const { isOpen } = props;
    this.state = this.getDefaultState(isOpen, []);
  }

  componentDidMount() {
    getGroupList(this.props.useFake)
      .then(groups =>
        this.setState({
          groups: [
            {
              key: "all",
              label: "All groups",
              total: 0
            }
          ].concat(this.convertGroups(groups))
        })
      )
      .catch(error => console.log(error));
  }

  componentDidUpdate(prevProps) {
    if (this.props.isOpen !== prevProps.isOpen)
      this.setState({ isOpen: this.props.isOpen });
  }

  convertGroups = groups => {
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

  convertUsers = users => {
    return users
      ? users.map(u => {
          return {
            key: u.id,
            groups: u.groups || [],
            label: u.displayName,
            email: u.email,
            position: u.title,
            avatarUrl: u.avatar
          };
        })
      : [];
  };

  loadNextPage = ({ startIndex, searchValue, currentGroup }) => {
    console.log(
      `loadNextPage(startIndex=${startIndex}, searchValue="${searchValue}", currentGroup="${currentGroup}")`
    );

    const pageCount = 100;

    this.setState({ isNextPageLoading: true }, () => {
      const filter = Filter.getDefault();
      filter.page = startIndex / pageCount;
      filter.pageCount = pageCount;

      if (searchValue) filter.search = searchValue;

      if (currentGroup && currentGroup !== "all") filter.group = currentGroup;

      getUserList(filter, this.props.useFake)
        .then(response => {
          const newOptions = (startIndex ? [...this.state.options] : []).concat(
            this.convertUsers(response.items)
          );

          this.setState({
            hasNextPage: newOptions.length < response.total,
            isNextPageLoading: false,
            options: newOptions
          });
        })
        .catch(error => console.log(error));
    });
  };

  getDefaultState = (isOpen, groups) => {
    return {
      isOpen: isOpen,
      options: [],
      groups,
      page: 0,
      hasNextPage: true,
      isNextPageLoading: false
    };
  };

  getOptionTooltipContent = index => {
    if (!index) return null;

    const { options } = this.state;

    const user = options[+index];

    if (!user) return null;

    // console.log("onOptionTooltipShow", index, user);

    return (
      <div
        style={{
          width: 253,
          minHeight: 63,
          display: "grid",
          gridTemplateColumns: "30px 1fr",
          gridTemplateRows: "1fr",
          gridColumnGap: 8
        }}
      >
        <Avatar
          size="small"
          role="user"
          source={user.avatarUrl}
          userName=""
          editing={false}
        />
        <div>
          <Text isBold={true} fontSize="13px" fontWeight={700}>
            {user.label}
          </Text>
          <Text
            color="#A3A9AE"
            fontSize="13px"
            style={{ paddingBottom: 8 }}
          >
            {user.email}
          </Text>
          <Text fontSize="13px" fontWeight={600}>{user.position}</Text>
        </div>
      </div>
    );
  };

  render() {
    const {
      isOpen,
      options,
      groups,
      selectedOptions,
      selectedGroups,
      hasNextPage,
      isNextPageLoading
    } = this.state;

    const { isMultiSelect, isDisabled, onSelect, size, onCancel } = this.props;

    return (
      <AdvancedSelector
        options={options}
        groups={groups}
        hasNextPage={hasNextPage}
        isNextPageLoading={isNextPageLoading}
        loadNextPage={this.loadNextPage}
        size={size}
        displayType={"auto"}
        selectedOptions={selectedOptions}
        selectedGroups={selectedGroups}
        isOpen={isOpen}
        isMultiSelect={isMultiSelect}
        isDisabled={isDisabled}
        searchPlaceHolderLabel={"Search users"}
        selectButtonLabel={"Add members"}
        selectAllLabel={"Select all"}
        groupsHeaderLabel={"Groups"}
        emptySearchOptionsLabel={"There are no users with such name"}
        emptyOptionsLabel={"There are no users"}
        loadingLabel={"Loading... Please wait..."}
        onSelect={onSelect}
        onSearchChanged={() => {
          //action("onSearchChanged")(value);
          this.setState({ options: [], hasNextPage: true });
        }}
        onGroupChanged={() => {
          //action("onGroupChanged")(group);
          this.setState({ options: [], hasNextPage: true });
        }}
        getOptionTooltipContent={this.getOptionTooltipContent}
        onCancel={onCancel}
      />
    );
  }
}

PeopleSelector.propTypes = {
  isOpen: PropTypes.bool,
  onSelect: PropTypes.func,
  onCancel: PropTypes.func,
  useFake: PropTypes.bool,
  isMultiSelect: PropTypes.bool,
  isDisabled: PropTypes.bool,
  size: PropTypes.oneOf(["full", "compact"])
};

PeopleSelector.defaultProps = {
  useFake: false,
  size: "full"
};

export default PeopleSelector;
