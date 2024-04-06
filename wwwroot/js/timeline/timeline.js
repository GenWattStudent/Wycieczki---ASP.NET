class Timeline {
  constructor(mealsModel) {
    this.mealsModel = mealsModel
    this.timelineItem = $('.checkpoint')
    this.timeLineProgress = $('#timeline-progress')
    this.currentTimelineProgress = $('#current-timeline-progress')
    this.currentTimelineProgressIndicator = $('#current-timeline-progress-indicator')
    this.currentMeal = null
    this.progress = 0
  }

  init() {
    this.timelineItem.each((index, element) => {
      const itemTop = element.offsetTop + element.offsetHeight / 2
      const lineDotIndicator = document.createElement('div')

      lineDotIndicator.classList.add('line-dot', 'bg-primary')
      lineDotIndicator.style.top = `${itemTop}px`

      this.timeLineProgress.append(lineDotIndicator)
    })

    const nextMeal = this.getNextMeal(this.mealsModel.$values)
    this.progress = this.getTimelineProgress(nextMeal)

    this.updateTimelineProgress()
  }

  getTimelineProgress(meal) {
    console.log(meal)
    if (!meal) {
      return 0
    }

    const mealIndex = this.mealsModel.$values.indexOf(meal)
    const now = new Date()
    const mealTime = new Date(meal.start)
    const diff = mealTime - now
    const mealPercent = ((mealIndex + 2) / this.mealsModel.$values.length) * 100
    console.log(mealIndex, this.mealsModel.$values.length)
    const nextMealProgress = (diff / mealTime) * 100
    console.log(nextMealProgress, mealPercent)
    const progress = mealPercent - nextMealProgress
    console.log(progress)
    return progress
  }

  getCurrentMeal(meals) {}

  getNextMeal(meals) {
    let nextMeal = null
    const now = new Date()

    for (let i = 0; i < meals.length; i++) {
      const mealTime = new Date(meals[i].start)
      if (mealTime > now) {
        nextMeal = meals[i]
        break
      }
    }

    return nextMeal
  }

  updateTimelineProgress() {
    console.log(this.progress)
    this.currentTimelineProgress.css('height', `${this.progress}%`)
    this.currentTimelineProgressIndicator.css('top', `${this.progress}%`)
  }
}
