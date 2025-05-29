using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using pmclient.Extensions;
using pmclient.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace pmclient.ViewModels;

public class GroupViewModel : ViewModelBase
{
    private Group _oldGroup;

    [Reactive] public int Id { get; set; }

    [Reactive] public string Title { get; set; }

    [Reactive] public char Image { get; set; }

    [Reactive] public int GroupId { get; set; }

    [Reactive] public bool IsEnable { get; set; } = true;

    public List<char> Images { get; } = ['\uf1c5', '\uf2ba', '\uf2bc', '\uf097', '\uf274', '\uf2c3', '\uf015'];

    public ReactiveCollection<CardViewModel> Cards { get; } = new();

    public ICommand DeleteCommand { get; set; }

    public ICommand SaveCommand { get; set; }

    public ICommand CancelCommand { get; set; }

    public ICommand EditCommand { get; set; }

    public GroupViewModel()
    {
    }

    public GroupViewModel(Group group)
    {
        SetData(group);
        EditCommand = ReactiveCommand.Create(Edit);
    }

    public Group GetGroup()
    {
        return _oldGroup;
    }

    private void Edit()
    {
        IsEnable = true;
    }

    public void Save()
    {
        SetData(new Group { Id = Id, GroupId = GroupId, Title = Title, Image = Image });
        IsEnable = false;
    }

    public void Cancel()
    {
        SetData(_oldGroup);
        IsEnable = false;
    }

    protected void SetData(Group newGroup)
    {
        _oldGroup = newGroup;
        Id = newGroup.Id;
        Title = newGroup.Title;
        GroupId = newGroup.GroupId;
        Image = newGroup.Image;
    }
}

public static class GroupViewModelExtensions
{
    public static GroupViewModel SetCards(this GroupViewModel vm, List<CardViewModel> cards)
    {
        vm.Cards.Replace(cards.Where(x => x.GroupId == vm.Id));

        return vm;
    }
}