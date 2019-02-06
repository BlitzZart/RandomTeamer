# RandomTeamer
A simple WPF application made to create randomized teams with the desired size. Plus, "incompatible/illegal" team member relation can be set.

## Known issues
The purpose of this project was to learn the WPF basics - working with XAML and MVVM. I spent less than half an hour to implement the actual random functionality. Hence, it is still work in progress - but might be finished in the following couple of weeks xD.

* The randomize process may fail if too many dependencies between the various users are set.
* If e.g. team size was set to 2 and afterward to 3 - it is not recognized. If it was 4 and the 3 it works fine.
