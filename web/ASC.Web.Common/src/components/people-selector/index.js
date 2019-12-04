import React from "react";
import PropTypes from "prop-types";
import { AdvancedSelector2, Avatar, Text } from "asc-web-components";
import { getUserList } from "../../api/people";
import { getGroupList } from "../../api/groups";
import Filter from "../../api/people/filter";
import { setClientBasePath } from "../../api/client";

class PeopleSelector extends React.Component {
    constructor(props) {
        super(props);

        const { isOpen } = props;
        this.state = this.getDefaultState(isOpen, []);
      }
    
      componentDidMount() {
        const PREFIX = "api";
        const VERSION = "2.0";
        const baseURL = `http://localhost:8092/${PREFIX}/${VERSION}`;
        setClientBasePath(baseURL);

        getGroupList()
          .then((groups) => this.setState({groups: [{
            key: "all",
            label: "All groups",
            total: 0
          }].concat(this.convertGroups(groups))}))
          .catch((error) => console.log(error));
      }

      componentDidUpdate(prevProps) {
        if(this.props.isOpen !== prevProps.isOpen)
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
          filter.page = startIndex/pageCount;
          filter.pageCount = pageCount;

          if(searchValue)
            filter.search = searchValue;

          if(currentGroup && currentGroup !== "all")
            filter.group = currentGroup;

          getUserList(filter)
            .then((response) => {

              const newOptions = (startIndex ? [...this.state.options] : []).concat(this.convertUsers(response.items));

              this.setState({
                hasNextPage: newOptions.length < response.total,
                isNextPageLoading: false,
                options: newOptions
              });
            })
            .catch((error) => console.log(error));
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
              <Text.Body isBold={true} fontSize={16}>
                {user.label}
              </Text.Body>
              <Text.Body
                color="#A3A9AE"
                fontSize={13}
                style={{ paddingBottom: 8 }}
              >
                {user.email}
              </Text.Body>
              <Text.Body fontSize={13}>{user.position}</Text.Body>
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

        const { 
            size, 
            displayTypes,
            isMultiSelect,
            isDisabled,
            onSelect
        } = this.props;

        return (
        <AdvancedSelector2
            options={options}
            groups={groups}
            hasNextPage={hasNextPage}
            isNextPageLoading={isNextPageLoading}
            loadNextPage={this.loadNextPage}
            size={size}
            displayType={displayTypes}
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
            loadingLabel={'Loading... Please wait...'}
            onSelect={onSelect}
            onSearchChanged={value => {
              //action("onSearchChanged")(value);
              this.setState({ options: [], hasNextPage: true });
            }}
            onGroupChanged={group => {
              //action("onGroupChanged")(group);
              this.setState({ options: [], hasNextPage: true });
            }}
            getOptionTooltipContent={this.getOptionTooltipContent}
      />);
    }
}

PeopleSelector.propTypes = {
  onSelect: PropTypes.func
}

PeopleSelector.defaultProps = {

}

export default PeopleSelector;